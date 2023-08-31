using System.ComponentModel;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Kamishibai;
using NAudio.CoreAudioApi;
using Reactive.Bindings.Disposables;
using Reactive.Bindings.Extensions;
using Specter;
using Specter.Business;

namespace Quietrum.ViewModel;

[Navigate]
// ReSharper disable once ClassNeverInstantiated.Global
public partial class MonitoringPageViewModel : ObservableObject, INavigatedAsyncAware, IDisposable
{
    private readonly CompositeDisposable _compositeDisposable = new();
    public RecordingConfig RecordingConfig { get; } = RecordingConfig.Default;

    private readonly IAudioInterfaceProvider _audioInterfaceProvider;
    private readonly ISettingsRepository _settingsRepository;
    private readonly IWaveRecordIndexRepository _waveRecordIndexRepository;
    
    [ObservableProperty] private bool _record;
    [ObservableProperty] private bool _withPlayback;
    [ObservableProperty] private bool _playback;
    [ObservableProperty] private string _recorderHost = string.Empty;
    [ObservableProperty] private string _recordName = string.Empty;
    [ObservableProperty] private TimeSpan _elapsed = TimeSpan.Zero;
    [ObservableProperty] private IList<DeviceViewModel> _devices = new List<DeviceViewModel>();
    [ObservableProperty] private IList<DeviceViewModel> _renderDevices = new List<DeviceViewModel>();
    [ObservableProperty] private IList<DeviceViewModel> _captureDevices = new List<DeviceViewModel>();
    [ObservableProperty] private DeviceViewModel? _playbackDevice;
    [ObservableProperty] private DeviceViewModel? _recordDevice;
    
    public MonitoringPageViewModel(
        [Inject] IAudioInterfaceProvider audioInterfaceProvider, 
        [Inject] ISettingsRepository settingsRepository, 
        [Inject] IWaveRecordIndexRepository waveRecordIndexRepository)
    {
        _audioInterfaceProvider = audioInterfaceProvider;
        _settingsRepository = settingsRepository;
        _waveRecordIndexRepository = waveRecordIndexRepository;
        this.ObserveProperty(x => x.Record)
            .Skip(1)
            .Subscribe(OnRecord)
            .AddTo(_compositeDisposable);
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
            .Subscribe(OnSelectedSpeaker)
            .AddTo(_compositeDisposable);
        this.ObserveProperty(x => x.RecorderHost)
            .Skip(1)
            .Subscribe(OnChangedRecorderHost)
            .AddTo(_compositeDisposable);
    }

    private async void OnChangedRecorderHost(string obj)
    {
        var settings = await _settingsRepository.LoadAsync();
        await _settingsRepository.SaveAsync(
            settings with { RecorderHost = RecorderHost });
    }

    private async void OnSelectedSpeaker(DeviceViewModel? speaker)
    {
        if(speaker is null) return;
        
        var settings = await _settingsRepository.LoadAsync();
        await _settingsRepository.SaveAsync(
            settings with { PlaybackDeviceId = PlaybackDevice?.Id });
    }

    private async void OnDeviceChanged(IList<DeviceViewModel> obj)
    {
        RenderDevices = Devices.Where(x => x.DataFlow == DataFlow.Render).ToList();
        PlaybackDevice = await GetPlaybackDevice(PlaybackDevice);

        CaptureDevices = Devices.Where(x => x.DataFlow == DataFlow.Capture).ToList();
        RecordDevice = await GetRecordDevice(RecordDevice);
    }

    /// <summary>
    /// 再生デバイスを取得する。
    /// </summary>
    /// <param name="playbackDevice"></param>
    /// <returns></returns>
    private async Task<DeviceViewModel?> GetPlaybackDevice(DeviceViewModel? playbackDevice)
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
            var device = CaptureDevices.SingleOrDefault(x => x.Id == recordDevice.Id);

            // 変更された録音デバイスの中に、選択済みの録音デバイスが存在した場合は変更しない。
            if (device is not null) return recordDevice;

            // 変更された録音デバイス内に存在しない＝取り外されたため、先頭の録音デバイスを選択状態とする。
            return CaptureDevices.FirstOrDefault();
        }

        // 過去に選択されていた録音デバイスのIDを取得する。
        var settings = await _settingsRepository.LoadAsync();
        return CaptureDevices.SingleOrDefault(x => x.Id == settings.PlaybackDeviceId)
               ?? CaptureDevices.FirstOrDefault();
    }

    private CancellationTokenSource _playBackCancellationTokenSource = new();
    
    private async void OnPlayBack(bool playBack)
    {
        if (playBack)
        {
            if (PlaybackDevice is null)
            {
                Playback = false;
                return;
            }

            _playBackCancellationTokenSource = new();
            await PlaybackDevice.PlayLoopingAsync(_playBackCancellationTokenSource.Token);
        }
        else
        {
            _playBackCancellationTokenSource.Cancel();
        }
        
    }


    private void OnRecord(bool record)
    {
        if (record)
        {
            StartRecording();
        }
        else
        {
            StopRecording();
        }
    }

    private void StartRecording()
    {
        const string recordNameFormat = "yyyy.MM.dd-HH.mm.ss";
        do
        {
            RecordName = DateTime.Now.ToString(recordNameFormat);
        } while (Directory.Exists(RecordName));

        var directory = new DirectoryInfo(RecordName);
        directory.Create();

        Devices
            .Where(x => x.Measure)
            .ToList()
            .ForEach(x => x.StartRecording(directory));
    }

    private void StopRecording()
    {
        Devices
            .Where(x => x.Measure)
            .ToList()
            .ForEach(x => x.StopRecording());
    }

    public async Task OnNavigatedAsync(PostForwardEventArgs args)
    {
        var settings = await _settingsRepository.LoadAsync();
        RecorderHost = settings.RecorderHost;
        var audioInterface = await _audioInterfaceProvider.ResolveAsync();
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
                        var microphone = new DeviceViewModel(device, RecordingConfig, _waveRecordIndexRepository);
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