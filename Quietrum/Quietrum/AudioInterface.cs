using System.Management;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using Reactive.Bindings;

namespace Quietrum;

/// <summary>
/// マイク・スピーカーなどを統合したオーディオ関連のインターフェース
/// </summary>
public class AudioInterface : IAudioInterface
{
    private readonly ManagementEventWatcher _watcher = new(
        new WqlEventQuery("__InstanceOperationEvent")
        {
            WithinInterval = TimeSpan.FromSeconds(3),
            Condition = "TargetInstance ISA 'Win32_SoundDevice'"
        });

    private readonly ISettingsRepository _settingsRepository;

    private readonly AudioDeviceWatcher _audioDeviceWatcher = new();
    
    /// <summary>
    /// 
    /// </summary>
    private readonly ReactivePropertySlim<IList<IMicrophone>> _microphones = new();

    /// <summary>
    /// すべてのマイクを扱うオーディオ インターフェースを作成する。
    /// </summary>
    /// <param name="settingsRepository"></param>
    public AudioInterface(ISettingsRepository settingsRepository)
    {
        _settingsRepository = settingsRepository;
        Microphones = _microphones.ToReadOnlyReactivePropertySlim()!;
        _microphones.Value = LoadMicrophones(_settingsRepository.Load()).ToList();
        _watcher.EventArrived += WatcherEventArrived;
        _watcher.Start();
    }

    /// <summary>
    /// 任意のマイクを扱うオーディオ インターフェースを作成する。
    /// </summary>
    /// <param name="settingsRepository"></param>
    /// <param name="microphones"></param>
    public AudioInterface(ISettingsRepository settingsRepository, params IMicrophone[] microphones)
    {
        _settingsRepository = settingsRepository;
        Microphones = _microphones.ToReadOnlyReactivePropertySlim()!;
        _microphones.Value = microphones;
        _watcher.EventArrived += WatcherEventArrived;
        _watcher.Start();
    }

    public ReadOnlyReactivePropertySlim<IList<IMicrophone>> Microphones { get; }

    /// <summary>
    /// すべてのマイクをロードする。
    /// </summary>
    /// <param name="settings"></param>
    /// <returns></returns>
    private IEnumerable<IMicrophone> LoadMicrophones(Settings settings)
    {
        using var enumerator = new MMDeviceEnumerator();
        var mmDevices = enumerator
            .EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active)
            .ToArray();
        try
        {
            for (var i = 0; i < WaveIn.DeviceCount; i++)
            {
                var capability = WaveIn.GetCapabilities(i);
                var name = capability.ProductName;
                // 名称が長いとWaveIn側の名前は途中までしか取得できないため、前方一致で判定する
                var mmDevice = mmDevices.SingleOrDefault(x => x.FriendlyName.StartsWith(name));
                if (mmDevice is not null)
                {
                    var microphoneId = new MicrophoneId(mmDevice.ID);
                    var alias = settings.Aliases.SingleOrDefault(x => x.Id == microphoneId)?.Name ?? mmDevice.FriendlyName;
                    yield return new Microphone(
                        microphoneId,
                        alias,
                        mmDevice.FriendlyName, 
                        i,
                        settings.DisabledMicrophones.NotContains(microphoneId) 
                            ? MicrophoneStatus.Enable 
                            : MicrophoneStatus.Disable);
                }
            }
        }
        finally
        {
            foreach (var mmDevice in mmDevices)
            {
                mmDevice.Dispose();
            }
        }

    }

    /// <summary>
    /// 状態が合致するマイクを取得する。
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    [Obsolete]
    public IEnumerable<IMicrophone> GetMicrophones(MicrophoneStatus status = MicrophoneStatus.Enable) =>
        _microphones.Value.Where(x => status.HasFlag(x.Status));

    /// <summary>
    /// 現在有効なスピーカーを取得する。
    /// </summary>
    /// <returns></returns>
    public async Task<ISpeaker> GetSpeakerAsync()
    {
        var settings = await _settingsRepository.LoadAsync();
        using var emurator = new MMDeviceEnumerator();
        // 明示的に指定されていればそれを、指定されていない場合、OSの出力先へ出力する。
        if (settings.SelectedSpeakerId is null)
        {
            using var mmDevice = emurator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            return new Speaker(new SpeakerId(mmDevice.ID), mmDevice.FriendlyName);
        }
        else
        {
            try
            {
                using var mmDevice = emurator.GetDevice(settings.SelectedSpeakerId?.AsPrimitive());
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
                        settings.Aliases,
                        settings.DisabledMicrophones,
                        null));
                using var mmDevice = emurator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
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
        using var emurator = new MMDeviceEnumerator();
        var mmDevices = emurator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
        foreach (var mmDevice in mmDevices)
            using (mmDevice)
            {
                yield return new Speaker(new SpeakerId(mmDevice.ID), mmDevice.FriendlyName);
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
                _microphones.Value = LoadMicrophones(_settingsRepository.Load()).ToList();
                break;
        }
    }

    class AudioDeviceWatcher
    {
        private readonly ManagementEventWatcher _watcher = new(
            new WqlEventQuery("__InstanceOperationEvent")
            {
                WithinInterval = TimeSpan.FromSeconds(3),
                Condition = "TargetInstance ISA 'Win32_SoundDevice'"
            });

        public AudioDeviceWatcher()
        {
            _watcher.EventArrived += WatcherEventArrived;
            _watcher.Start();
        }

        private void WatcherEventArrived(object sender, EventArrivedEventArgs e)
        {
            var eventType = e.NewEvent.ClassPath.ClassName;
            var targetInstance = e.NewEvent["TargetInstance"] as ManagementBaseObject;

            if (targetInstance != null)
            {
                var deviceName = targetInstance["Name"] as string;
                if (eventType == "__InstanceCreationEvent")
                {
                    Console.WriteLine($"Device connected: {deviceName}");
                }
                else if (eventType == "__InstanceDeletionEvent")
                {
                    Console.WriteLine($"Device disconnected: {deviceName}");
                }
            }
        }

        public void Stop()
        {
            _watcher.Stop();
        }
    }

}