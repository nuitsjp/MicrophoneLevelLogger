namespace Specter.Business;

/// <summary>
/// MicrophoneLevelLoggerの各種設定
/// </summary>
public class Settings
{
    private readonly List<MicrophoneConfig> _microphoneConfigs;

    /// <summary>
    /// インスタンスを生成する。
    /// </summary>
    /// <param name="recorderHost"></param>
    /// <param name="recordingSpan"></param>
    /// <param name="playbackDeviceId"></param>
    /// <param name="microphoneConfigs"></param>
    public Settings(
        string recorderHost,
        TimeSpan recordingSpan,
        DeviceId? playbackDeviceId, 
        IReadOnlyList<MicrophoneConfig> microphoneConfigs)
    {
        RecorderHost = recorderHost;
        RecordingSpan = recordingSpan;
        PlaybackDeviceId = playbackDeviceId;
        _microphoneConfigs = microphoneConfigs.ToList();
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
    public IReadOnlyList<MicrophoneConfig> MicrophoneConfigs => _microphoneConfigs;

    public DeviceId? PlaybackDeviceId { get; }

    public bool TryGetMicrophoneConfig(DeviceId id, out MicrophoneConfig microphoneConfig)
    {
        var config = _microphoneConfigs.SingleOrDefault(x => x.Id == id);
        microphoneConfig = config!;
        return config is not null;
    }

    public MicrophoneConfig GetMicrophoneConfig(DeviceId id)
    {
        return _microphoneConfigs.Single(x => x.Id == id);
    }

    public void AddMicrophoneConfig(MicrophoneConfig microphoneConfig) => _microphoneConfigs.Add(microphoneConfig);
}