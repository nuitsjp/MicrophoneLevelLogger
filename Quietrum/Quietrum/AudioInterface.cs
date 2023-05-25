using System.Management;
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

    /// <summary>
    /// 
    /// </summary>
    [ObservableProperty]
    private IReadOnlyList<IMicrophone> _microphones;

    private Settings _settings;
    /// <summary>
    /// すべてのマイクを扱うオーディオ インターフェースを作成する。
    /// </summary>
    /// <param name="settingsRepository"></param>
    /// <param name="settings"></param>
    public AudioInterface(ISettingsRepository settingsRepository, Settings settings)
    {
        _settingsRepository = settingsRepository;
        _settings = settings;
        _microphones = LoadMicrophones().ToList();
        _watcher.EventArrived += WatcherEventArrived;
        _watcher.Start();
    }

    /// <summary>
    /// すべてのマイクをロードする。
    /// </summary>
    /// <returns></returns>
    private IEnumerable<IMicrophone> LoadMicrophones()
    {
        using var enumerator = new MMDeviceEnumerator();
        var mmDevices = enumerator
            .EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active)
            .ToArray();
        var modified = false;
        foreach (var mmDevice in mmDevices)
        {
            var microphoneId = new MicrophoneId(mmDevice.ID);
            if (_settings.TryGetMicrophoneConfig(microphoneId, out var microphoneConfig) is false)
            {
                modified = true;
                microphoneConfig = new MicrophoneConfig(microphoneId, mmDevice.FriendlyName, true);
                _settings.AddMicrophoneConfig(microphoneConfig);
            }

            yield return new Microphone(
                    microphoneId,
                    microphoneConfig.Name,
                    mmDevice.FriendlyName, 
                    microphoneConfig.Measure,
                    mmDevice);
        }

        if (modified)
        {
            _settingsRepository.SaveAsync(_settings);
        }
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
    
    private void WatcherEventArrived(object sender, EventArrivedEventArgs e)
    {
        if (e.NewEvent["TargetInstance"] is not ManagementBaseObject) return;
        
        var eventType = e.NewEvent.ClassPath.ClassName;
        switch (eventType)
        {
            case "__InstanceCreationEvent":
            case "__InstanceDeletionEvent":
                Microphones = LoadMicrophones().ToList();
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