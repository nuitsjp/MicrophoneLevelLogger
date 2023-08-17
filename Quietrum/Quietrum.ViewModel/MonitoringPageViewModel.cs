using System.Collections.Specialized;
using System.ComponentModel;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Kamishibai;
using NAudio.CoreAudioApi;
using Reactive.Bindings;
using Reactive.Bindings.Disposables;
using Reactive.Bindings.Extensions;
using ScottPlot;
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
    
    [ObservableProperty] private bool _monitor = true;
    [ObservableProperty] private bool _record;
    [ObservableProperty] private bool _connectPlayer;
    [ObservableProperty] private bool _connectRecorder;
    [ObservableProperty] private string _playerHost;
    [ObservableProperty] private string _recorderHost;
    [ObservableProperty] private string _recordName = string.Empty;
    [ObservableProperty] private TimeSpan _elapsed = TimeSpan.Zero;
    [ObservableProperty] private IList<DeviceViewModel> _devices = new List<DeviceViewModel>();
    [ObservableProperty] private IList<DeviceViewModel> _speakers = new List<DeviceViewModel>();
    [ObservableProperty] private DeviceViewModel? _selectedSpeaker;
    
    public MonitoringPageViewModel(
        [Inject] IAudioInterfaceProvider audioInterfaceProvider, 
        [Inject] ISettingsRepository settingsRepository)
    {
        _audioInterfaceProvider = audioInterfaceProvider;
        _settingsRepository = settingsRepository;
        this.ObserveProperty(x => x.Monitor)
            .Skip(1)
            .Subscribe(OnMonitor)
            .AddTo(_compositeDisposable);
        this.ObserveProperty(x => x.Record)
            .Skip(1)
            .Subscribe(OnRecord)
            .AddTo(_compositeDisposable);
        this.ObserveProperty(x => x.ConnectPlayer)
            .Skip(1)
            .Subscribe(OnConnectPlayer)
            .AddTo(_compositeDisposable);
        this.ObserveProperty(x => x.ConnectRecorder)
            .Skip(1)
            .Subscribe(OnConnect)
            .AddTo(_compositeDisposable);
        this.ObserveProperty(x => x.Devices)
            .Skip(1)
            .Subscribe(OnDeviceChanged)
            .AddTo(_compositeDisposable);
        this.ObserveProperty(x => x.SelectedSpeaker)
            .Skip(1)
            .Subscribe(OnSelectedSpeaker)
            .AddTo(_compositeDisposable);
        this.ObserveProperty(x => x.PlayerHost)
            .Skip(1)
            .Subscribe(OnChangedMediaPlayerHost)
            .AddTo(_compositeDisposable);
        this.ObserveProperty(x => x.RecorderHost)
            .Skip(1)
            .Subscribe(OnChangedRecorderHost)
            .AddTo(_compositeDisposable);
    }

    private void OnConnectPlayer(bool connectPlayer)
    {
        throw new NotImplementedException();
    }

    private async void OnChangedRecorderHost(string obj)
    {
        var settings = await _settingsRepository.LoadAsync();
        await _settingsRepository.SaveAsync(
            new(
                settings.MediaPlayerHost,
                RecorderHost,
                settings.RecordingSpan,
                settings.IsEnableRemotePlaying,
                settings.IsEnableRemoteRecording,
                settings.SelectedSpeakerId,
                settings.MicrophoneConfigs));
    }

    private async void OnChangedMediaPlayerHost(string playerHost)
    {
        var settings = await _settingsRepository.LoadAsync();
        await _settingsRepository.SaveAsync(
            new(
                PlayerHost,
                settings.RecorderHost,
                settings.RecordingSpan,
                settings.IsEnableRemotePlaying,
                settings.IsEnableRemoteRecording,
                settings.SelectedSpeakerId,
                settings.MicrophoneConfigs));
    }

    private async void OnSelectedSpeaker(DeviceViewModel speaker)
    {
        if(speaker is null) return;
        
        var settings = await _settingsRepository.LoadAsync();
        await _settingsRepository.SaveAsync(
            new(
                settings.MediaPlayerHost,
                settings.RecorderHost,
                settings.RecordingSpan,
                settings.IsEnableRemotePlaying,
                settings.IsEnableRemoteRecording,
                speaker.Id,
                settings.MicrophoneConfigs));

    }

    private async void OnDeviceChanged(IList<DeviceViewModel> obj)
    {
        Speakers = Devices.Where(x => x.DataFlow == DataFlow.Render).ToList();
        if (SelectedSpeaker is not null)
        {
            // スピーカーがすでに選択済みの場合
            var speaker = Speakers.SingleOrDefault(x => x.Id == SelectedSpeaker.Id);
            
            // 変更されtらスピーカーの中に、選択済みのスピーカーが存在した場合は変更しない。
            if (speaker is not null) return;
            
            // 変更されたスピーカー内に存在しない＝取り外されたため、先頭のスピーカーを選択状態とする。
            SelectedSpeaker = Speakers.FirstOrDefault();
        }

        // 過去に選択されていたスピーカーのIDを取得する。
        var settings = await _settingsRepository.LoadAsync();
        SelectedSpeaker = 
            Speakers.SingleOrDefault(x => x.Id == settings.SelectedSpeakerId) 
            ?? Speakers.FirstOrDefault();
    }

    private void OnConnect(bool connect)
    {
        if (connect)
        {
            SelectedSpeaker?.Connect();
        }
        else
        {
            SelectedSpeaker?.Disconnect();
        }
    }

    private void OnMonitor(bool monitor)
    {
        if (monitor)
        {
            Devices
                .Where(x => x.Measure)
                .ToList()
                .ForEach(x => x.StartMonitoring());
        }
        else
        {
            Devices
                .Where(x => x.Measure)
                .ToList()
                .ForEach(x => x.StopMonitoring());
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
        PlayerHost = settings.MediaPlayerHost;
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