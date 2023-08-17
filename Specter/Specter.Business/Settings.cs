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
    /// <param name="mediaPlayerHost"></param>
    /// <param name="recorderHost"></param>
    /// <param name="recordingSpan"></param>
    /// <param name="isEnableRemotePlaying"></param>
    /// <param name="isEnableRemoteRecording"></param>
    /// <param name="selectedSpeakerId"></param>
    /// <param name="microphoneConfigs"></param>
    public Settings(
        string mediaPlayerHost,
        string recorderHost,
        TimeSpan recordingSpan,
        bool isEnableRemotePlaying,
        bool isEnableRemoteRecording, 
        DeviceId? selectedSpeakerId, 
        IReadOnlyList<MicrophoneConfig> microphoneConfigs)
    {
        MediaPlayerHost = mediaPlayerHost;
        RecorderHost = recorderHost;
        RecordingSpan = recordingSpan;
        IsEnableRemotePlaying = isEnableRemotePlaying;
        IsEnableRemoteRecording = isEnableRemoteRecording;
        SelectedSpeakerId = selectedSpeakerId;
        _microphoneConfigs = microphoneConfigs.ToList();
    }

    /// <summary>
    /// 録音時間
    /// </summary>
    public TimeSpan RecordingSpan { get; }
    /// <summary>
    /// リモート再生を有効とする
    /// </summary>
    public bool IsEnableRemotePlaying { get; }
    /// <summary>
    /// リモート再生ホスト
    /// </summary>
    public string MediaPlayerHost { get; }
    /// <summary>
    /// リモート録音を有効とする
    /// </summary>
    public bool IsEnableRemoteRecording { get; }
    /// <summary>
    /// リモート録音ホスト
    /// </summary>
    public string RecorderHost { get; }
    /// <summary>
    /// マイク設定
    /// </summary>
    public IReadOnlyList<MicrophoneConfig> MicrophoneConfigs => _microphoneConfigs;

    public DeviceId? SelectedSpeakerId { get; }

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