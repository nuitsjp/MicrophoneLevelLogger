using System.ComponentModel;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kamishibai;
using NAudio.CoreAudioApi;
using Reactive.Bindings.Disposables;
using Reactive.Bindings.Extensions;

namespace Specter.ViewModel.MonitoringPage;

[Navigate]
// ReSharper disable once ClassNeverInstantiated.Global
public partial class MonitoringPageViewModel : ObservableObject, INavigatedAsyncAware, IDisposable
{
    private readonly CompositeDisposable _compositeDisposable = new();
    public RecordingConfig RecordingConfig { get; } = RecordingConfig.Default;

    private readonly IAudioInterfaceProvider _audioInterfaceProvider;
    private readonly IAudioRecordInterface _audioRecordInterface;
    private readonly ISettingsRepository _settingsRepository;
    
    // [ObservableProperty] private bool _record;
    [ObservableProperty] private bool _withPlayback;
    [ObservableProperty] private bool _playback;
    [ObservableProperty] private string _recorderHost = string.Empty;
    [ObservableProperty] private string _recordName = string.Empty;
    [ObservableProperty] private TimeSpan _elapsed = TimeSpan.Zero;
    [ObservableProperty] private IList<DeviceViewModel> _devices = new List<DeviceViewModel>();
    [ObservableProperty] private IList<RenderDeviceViewModel> _renderDevices = new List<RenderDeviceViewModel>();
    [ObservableProperty] private IList<DeviceViewModel> _measureDevices = new List<DeviceViewModel>();
    [ObservableProperty] private RenderDeviceViewModel? _playbackDevice;
    [ObservableProperty] private DeviceViewModel? _recordDevice;
    [ObservableProperty] private RecordingMethod _selectedDirection = RecordingMethod.RecordingMethods.First();
    [ObservableProperty] private int _recordingSpan;
    
    public MonitoringPageViewModel(
        [Inject] IAudioInterfaceProvider audioInterfaceProvider, 
        [Inject] IAudioRecordInterface audioRecordInterface, 
        [Inject] ISettingsRepository settingsRepository)
    {
        _audioInterfaceProvider = audioInterfaceProvider;
        _audioRecordInterface = audioRecordInterface;
        _settingsRepository = settingsRepository;
        this.ObserveProperty(x => x.Playback)
            .Skip(1)
            .Subscribe(OnPlayBack)
            .AddTo(_compositeDisposable);
        this.ObserveProperty(x => x.Devices)
            .Skip(1)
            .Subscribe(OnDeviceChanged)
            .AddTo(_compositeDisposable);
        this.ObserveProperty(x => x.PlaybackDevice)
            .Skip(1)
            .Subscribe(OnSelectedRenderDevice)
            .AddTo(_compositeDisposable);
        this.ObserveProperty(x => x.RecorderHost)
            .Skip(1)
            .Subscribe(OnChangedRecorderHost)
            .AddTo(_compositeDisposable);
        this.ObserveProperty(x => x.RecordingSpan)
            .Skip(1)
            .Subscribe(OnRecordingSpan)
            .AddTo(_compositeDisposable);
    }

    private async void OnRecordingSpan(int recordingSpan)
    {
        var settings = await _settingsRepository.LoadAsync();
        await _settingsRepository.SaveAsync(
            settings with { RecordingSpan = TimeSpan.FromSeconds(recordingSpan)});
    }

    public IReadOnlyList<RecordingMethod> RecordingMethods { get; } = RecordingMethod.RecordingMethods;

    private async void OnChangedRecorderHost(string obj)
    {
        var settings = await _settingsRepository.LoadAsync();
        await _settingsRepository.SaveAsync(
            settings with { RecorderHost = RecorderHost });
    }

    private async void OnSelectedRenderDevice(RenderDeviceViewModel? renderDevice)
    {
        if(renderDevice is null) return;
        
        var settings = await _settingsRepository.LoadAsync();
        await _settingsRepository.SaveAsync(
            settings with { PlaybackDeviceId = PlaybackDevice?.Id });
    }

    private async void OnDeviceChanged(IList<DeviceViewModel> obj)
    {
        RenderDevices = Devices
            .Where(x => x.DataFlow == DataFlow.Render)
            .Select(x => new RenderDeviceViewModel((IRenderDevice)x.Device))
            .ToList();
        PlaybackDevice = await GetPlaybackDevice(PlaybackDevice);

        MeasureDevices = Devices
            .Where(x => x is { DataFlow: DataFlow.Capture, Measure: true })
            .ToList();
        RecordDevice = await GetRecordDevice(RecordDevice);
    }

    /// <summary>
    /// 再生デバイスを取得する。
    /// </summary>
    /// <param name="playbackDevice"></param>
    /// <returns></returns>
    private async Task<RenderDeviceViewModel?> GetPlaybackDevice(RenderDeviceViewModel? playbackDevice)
    {
        if (playbackDevice is not null)
        {
            // 再生デバイスがすでに選択済みの場合
            var device = RenderDevices.SingleOrDefault(x => x.Id == playbackDevice.Id);

            // 変更されtら再生デバイスの中に、選択済みの再生デバイスが存在した場合は変更しない。
            if (device is not null) return playbackDevice;

            // 変更された再生デバイス内に存在しない＝取り外されたため、先頭の再生デバイスを選択状態とする。
            return RenderDevices.FirstOrDefault();
        }

        // 過去に選択されていた再生デバイスのIDを取得する。
        var settings = await _settingsRepository.LoadAsync();
        return RenderDevices.SingleOrDefault(x => x.Id == settings.PlaybackDeviceId)
               ?? RenderDevices.FirstOrDefault();
    }

    /// <summary>
    /// 録音デバイスを取得する。
    /// </summary>
    /// <param name="recordDevice"></param>
    /// <returns></returns>
    private async Task<DeviceViewModel?> GetRecordDevice(DeviceViewModel? recordDevice)
    {
        if (recordDevice is not null)
        {
            // 録音デバイスがすでに選択済みの場合
            var device = MeasureDevices.SingleOrDefault(x => x.Id == recordDevice.Id);

            // 変更された録音デバイスの中に、選択済みの録音デバイスが存在した場合は変更しない。
            if (device is not null) return recordDevice;

            // 変更された録音デバイス内に存在しない＝取り外されたため、先頭の録音デバイスを選択状態とする。
            return MeasureDevices.FirstOrDefault();
        }

        // 過去に選択されていた録音デバイスのIDを取得する。
        var settings = await _settingsRepository.LoadAsync();
        return MeasureDevices.SingleOrDefault(x => x.Id == settings.PlaybackDeviceId)
               ?? MeasureDevices.FirstOrDefault();
    }

    private CancellationTokenSource _playBackCancellationTokenSource = new();
    
    private void OnPlayBack(bool playBack)
    {
        if (playBack)
        {
            if (PlaybackDevice is null)
            {
                Playback = false;
                return;
            }

            _playBackCancellationTokenSource = new();
            PlaybackDevice.PlayLooping(_playBackCancellationTokenSource.Token);
        }
        else
        {
            _playBackCancellationTokenSource.Cancel();
        }
        
    }

    /// <summary>
    /// 実行中の録音
    /// </summary>
    private IAudioRecording? _audioRecording;
    
    /// <summary>
    /// 実行中の録音を取得・設定する
    /// </summary>
    private IAudioRecording? AudioRecording
    {
        get => _audioRecording;
        set
        {
            // 録音が更新された場合、Recordingの変更を通知する
            _audioRecording = value;
            OnPropertyChanged(nameof(Recording));
        }
    }

    /// <summary>
    /// 録音状態を取得する
    /// </summary>
    public bool Recording => _audioRecording is not null;

    /// <summary>
    /// 録音の進捗を取得する
    /// </summary>
    public int RecordingProgress => (int)((DateTime.Now - _startRecordingTime).TotalSeconds * 100 / RecordingSpan);

    /// <summary>
    /// 録音開始時刻
    /// </summary>
    private DateTime _startRecordingTime;

    /// <summary>
    /// 録音停止タイマー
    /// </summary>
    private DispatcherTimer _recordTimer = new();
    /// <summary>
    /// 進捗更新タイマー
    /// </summary>
    private DispatcherTimer _updateProgressTimer = new();

    /// <summary>
    /// 録音の開始・停止を行うコマンド
    /// </summary>
    [RelayCommand]
    private void Record()
    {
        if (Recording is false)
        {
            // 録音が未実施の状態でコマンドが実行された場合、録音を開始する
            StartRecording();
        }
        else
        {
            // 録音中の場合、録音を停止する
            StopRecording();
        }
    }

    /// <summary>
    /// 録音を開始する
    /// </summary>
    private void StartRecording()
    {
        // 録音を開始する
        AudioRecording = _audioRecordInterface
            .BeginRecording(
                RecordDevice!.Device,
                SelectedDirection,
                Devices
                    .Where(x => x.Measure)
                    .Select(x => x.Device),
                PlaybackDevice?.Device as IRenderDevice,
                RecordingConfig.WaveFormat);

        // 録音開始時刻を記録する
        _startRecordingTime = DateTime.Now;

        // 進捗更新タイマーを起動する
        _updateProgressTimer = new() { Interval = TimeSpan.FromMilliseconds(100) };
        _updateProgressTimer.Tick += (_, _) => OnPropertyChanged(nameof(RecordingProgress));
        _updateProgressTimer.Start();

        // 録音タイマーを起動する
        _recordTimer = new() { Interval = TimeSpan.FromSeconds(RecordingSpan) };
        _recordTimer.Tick += (_, _) => StopRecording();
        _recordTimer.Start();
    }

    // 録音を停止する
    private async void StopRecording()
    {
        // 各種タイマーを停止する
        if (_recordTimer.IsEnabled) _recordTimer.Stop();
        if (_updateProgressTimer.IsEnabled) _updateProgressTimer.Stop();
        
        // 録音を停止する
        if(AudioRecording is null) return;
        
        await AudioRecording.EndRecordingAsync();
        AudioRecording = null;
    }

    public async Task OnNavigatedAsync(PostForwardEventArgs args)
    {
        var settings = await _settingsRepository.LoadAsync();
        RecorderHost = settings.RecorderHost;
        RecordingSpan = (int)settings.RecordingSpan.TotalSeconds;
        var audioInterface = _audioInterfaceProvider.Resolve();
        audioInterface.Devices
            .CollectionChangedAsObservable()
            .ObserveOn(CurrentThreadScheduler.Instance)
            .Subscribe(eventArgs =>
            {
                List<DeviceViewModel> newViewModels = new(Devices);
                if (eventArgs.NewItems is not null)
                {
                    // 接続されたIMicrophoneを追加する。
                    foreach (IDevice device in eventArgs.NewItems!)
                    {
                        var microphone = new DeviceViewModel(device, RecordingConfig);
                        microphone.PropertyChanged += MicrophoneOnPropertyChanged;
                        if (microphone.Measure)
                        {
                            microphone.StartMonitoring();
                        }
                        newViewModels.Add(microphone);
                    }
                }

                if (eventArgs.OldItems is not null)
                {
                    foreach (IDevice device in eventArgs.OldItems)
                    {
                        var viewModel = Devices.Single(x => x.Id == device.Id);
                        viewModel.PropertyChanged -= MicrophoneOnPropertyChanged;
                        newViewModels.Remove(viewModel);
                    }
                }
                Devices = newViewModels
                    .OrderBy(x => x.DataFlow.ToString())
                    .ThenBy(x => x.Name)
                    .ToList();
            });

        await audioInterface.ActivateAsync();
    }

    private void MicrophoneOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(nameof(Devices));
    }

    public void Dispose()
    {
        Devices.Dispose();
        _compositeDisposable.Dispose();
    }
}