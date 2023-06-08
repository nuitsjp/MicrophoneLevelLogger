using System.ComponentModel;
using System.Management;
using System.Reactive.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using NAudio.CoreAudioApi;

namespace Quietrum;

/// <summary>
/// マイク・スピーカーなどを統合したオーディオ関連のインターフェース
/// </summary>
public partial class AudioInterface : ObservableObject, IAudioInterface
{
    private readonly ManagementEventWatcher _watcher = new(
        new WqlEventQuery("__InstanceOperationEvent")
        {
            WithinInterval = TimeSpan.FromSeconds(3),
            Condition = "TargetInstance ISA 'Win32_SoundDevice'"
        });

    private readonly ISettingsRepository _settingsRepository;

    private readonly IRemoteDeviceServer _remoteDeviceServer;

    /// <summary>
    /// 
    /// </summary>
    [ObservableProperty]
    private IReadOnlyList<IDevice> _microphones = new List<IDevice>();

    private readonly List<RemoteDevice> _remoteDevices = new();

    private Settings _settings;

    /// <summary>
    /// すべてのマイクを扱うオーディオ インターフェースを作成する。
    /// </summary>
    /// <param name="settingsRepository"></param>
    /// <param name="remoteDeviceServer"></param>
    internal AudioInterface(
        ISettingsRepository settingsRepository, 
        IRemoteDeviceServer remoteDeviceServer)
    {
        _settingsRepository = settingsRepository;
        _remoteDeviceServer = remoteDeviceServer;
        _settings = default!;
        _watcher.EventArrived += WatcherEventArrived;
        _watcher.Start();
        _remoteDeviceServer.RemoteDevicesChanged += (_, _) => ReloadMicrophonesAsync().ConfigureAwait(true);
    }

    public async Task ActivateAsync()
    {
        _settings = await _settingsRepository.LoadAsync();
        _remoteDeviceServer.Activate();
        await ReloadMicrophonesAsync();
    }

    /// <summary>
    /// すべてのマイクをリロードする。
    /// </summary>
    /// <returns></returns>
    private async Task ReloadMicrophonesAsync()
    {
        var newMicrophones = new List<IDevice>();
        using var enumerator = new MMDeviceEnumerator();
        var mmDevices = enumerator
            .EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active)
            .ToList();
        var modified = false;
        // 新たに接続されたマイクを追加する。
        foreach (var mmDevice in mmDevices)
        {
            var microphoneId = new DeviceId(mmDevice.ID);
            if (Microphones.TryGet(x => x.Id == microphoneId, out var microphone))
            {
                // すでに登録されているマイクだった場合、mmDeviceのリソースは開放する。
                mmDevice.Dispose();
            }
            else
            {
                // 新たに接続されたマイクだった場合
                if (_settings.TryGetMicrophoneConfig(microphoneId, out var microphoneConfig) is false)
                {
                    modified = true;
                    microphoneConfig = new MicrophoneConfig(microphoneId, mmDevice.FriendlyName, true);
                    _settings.AddMicrophoneConfig(microphoneConfig);
                }

                microphone = new Device(
                    microphoneId,
                    microphoneConfig.Name,
                    mmDevice.FriendlyName, 
                    microphoneConfig.Measure,
                    mmDevice);
                microphone.PropertyChanged += MicrophoneOnPropertyChanged;
            }
            newMicrophones.Add(microphone);
        }

        // 取り外されたマイクがあればリソースを開放する
        foreach (var microphone in Microphones.ToArray())
        {
            if (newMicrophones.NotContains(x => x.Id == microphone.Id))
            {
                // 取り外されたマイクのリソースを開放する
                microphone.Dispose();
            }
        }

        if (modified)
        {
            await _settingsRepository.SaveAsync(_settings);
        }

        Microphones = newMicrophones;
    }

    private async void MicrophoneOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not IDevice microphone) return;
        if(e.PropertyName is not (
           nameof(IDevice.Name) 
           or nameof(IDevice.Measure))) return;
        
        var config = _settings.GetMicrophoneConfig(microphone.Id);
        config.Name = microphone.Name;
        config.Measure = microphone.Measure;
        await _settingsRepository.SaveAsync(_settings);
    }

    /// <summary>
    /// 現在有効なスピーカーを取得する。
    /// </summary>
    /// <returns></returns>
    public async Task<ISpeaker> GetSpeakerAsync()
    {
        var settings = await _settingsRepository.LoadAsync();
        using var enumerator = new MMDeviceEnumerator();
        // 明示的に指定されていればそれを、指定されていない場合、OSの出力先へ出力する。
        if (settings.SelectedSpeakerId is null)
        {
            using var mmDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            return new Speaker(new SpeakerId(mmDevice.ID), mmDevice.FriendlyName);
        }
        else
        {
            try
            {
                using var mmDevice = enumerator.GetDevice(settings.SelectedSpeakerId?.AsPrimitive());
                return new Speaker(new SpeakerId(mmDevice.ID), mmDevice.FriendlyName);
            }
            catch
            {
                // スピーカーを外したりすると、デバイスが見つからなくなるのでその場合は削除してデフォルトを返す。
                await _settingsRepository.SaveAsync(
                    new(
                        settings.MediaPlayerHost,
                        settings.RecorderHost,
                        settings.RecordingSpan,
                        settings.IsEnableRemotePlaying,
                        settings.IsEnableRemoteRecording,
                        null,
                        settings.MicrophoneConfigs));
                using var mmDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                return new Speaker(new SpeakerId(mmDevice.ID), mmDevice.FriendlyName);
            }
        }
    }

    /// <summary>
    /// すべてのスピーカーを取得する。
    /// </summary>
    /// <returns></returns>
    public IEnumerable<ISpeaker> GetSpeakers()
    {
        using var enumerator = new MMDeviceEnumerator();
        var mmDevices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
        foreach (var mmDevice in mmDevices)
        {
            using (mmDevice)
            {
                yield return new Speaker(new SpeakerId(mmDevice.ID), mmDevice.FriendlyName);
            }
        }
    }
    
    private async void WatcherEventArrived(object sender, EventArrivedEventArgs e)
    {
        if (e.NewEvent["TargetInstance"] is not ManagementBaseObject) return;
        
        var eventType = e.NewEvent.ClassPath.ClassName;
        switch (eventType)
        {
            case "__InstanceCreationEvent":
            case "__InstanceDeletionEvent":
                await ReloadMicrophonesAsync();
                break;
        }
    }

    public void Dispose()
    {
        Microphones.Dispose();
        _watcher.Stop();
        _watcher.Dispose();
    }
}