using NAudio.MediaFoundation;

namespace MicrophoneLevelLogger;

public class Settings
{
    private readonly List<Alias> _aliases;
    private readonly List<MicrophoneId> _excludeMicrophones;

    public Settings(
        string mediaPlayerHost,
        string recorderHost,
        TimeSpan recordingSpan,
        bool isEnableRemotePlaying,
        bool isEnableRemoteRecording, 
        IReadOnlyList<Alias> aliases, 
        IReadOnlyList<MicrophoneId> excludeMicrophones)
    {
        MediaPlayerHost = mediaPlayerHost;
        RecorderHost = recorderHost;
        RecordingSpan = recordingSpan;
        IsEnableRemotePlaying = isEnableRemotePlaying;
        IsEnableRemoteRecording = isEnableRemoteRecording;
        _aliases = aliases.ToList();
        _excludeMicrophones = excludeMicrophones.ToList();
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

    public IReadOnlyList<Alias> Aliases => _aliases;

    public IReadOnlyList<MicrophoneId> ExcludeMicrophones => _excludeMicrophones;

    public void UpdateAlias(Alias alias)
    {
        _aliases.Remove(x => x.Id == alias.Id);
        _aliases.Add(alias);
    }

    public void RemoveAlias(Alias alias)
    {
        _aliases.Remove(alias);
    }

    public void ExcludeMicrophone(MicrophoneId id)
    {
        if (_excludeMicrophones.Contains(id))
        {
            return;
        }
        _excludeMicrophones.Add(id);
    }
}