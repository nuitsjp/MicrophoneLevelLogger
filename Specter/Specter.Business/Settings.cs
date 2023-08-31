namespace Specter.Business;

/// <summary>
/// MicrophoneLevelLoggerの各種設定
/// </summary>
public class Settings
{
    private readonly List<DeviceConfig> _deviceConfigs;

    /// <summary>
    /// インスタンスを生成する。
    /// </summary>
    /// <param name="recorderHost"></param>
    /// <param name="recordingSpan"></param>
    /// <param name="playbackDeviceId"></param>
    /// <param name="deviceConfigs"></param>
    public Settings(
        string recorderHost,
        TimeSpan recordingSpan,
        DeviceId? playbackDeviceId, 
        IReadOnlyList<DeviceConfig> deviceConfigs)
    {
        RecorderHost = recorderHost;
        RecordingSpan = recordingSpan;
        PlaybackDeviceId = playbackDeviceId;
        _deviceConfigs = deviceConfigs.ToList();
    }

    /// <summary>
    /// 録音時間
    /// </summary>
    public TimeSpan RecordingSpan { get; }
    /// <summary>
    /// リモート録音ホスト
    /// </summary>
    public string RecorderHost { get; }
    /// <summary>
    /// マイク設定
    /// </summary>
    public IReadOnlyList<DeviceConfig> DeviceConfigs => _deviceConfigs;

    public DeviceId? PlaybackDeviceId { get; }

    public bool TryGetMicrophoneConfig(DeviceId id, out DeviceConfig deviceConfig)
    {
        var config = _deviceConfigs.SingleOrDefault(x => x.Id == id);
        deviceConfig = config!;
        return config is not null;
    }

    public DeviceConfig GetMicrophoneConfig(DeviceId id)
    {
        return _deviceConfigs.Single(x => x.Id == id);
    }

    public void AddMicrophoneConfig(DeviceConfig deviceConfig) => _deviceConfigs.Add(deviceConfig);
}