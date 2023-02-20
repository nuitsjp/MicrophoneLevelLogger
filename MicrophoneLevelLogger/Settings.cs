using NAudio.MediaFoundation;

namespace MicrophoneLevelLogger;

public class Settings
{
    public Settings(
        string mediaPlayerHost,
        string recorderHost,
        TimeSpan recordingSpan,
        bool isEnableRemotePlaying,
        bool isEnableRemoteRecording)
    {
        MediaPlayerHost = mediaPlayerHost;
        RecorderHost = recorderHost;
        RecordingSpan = recordingSpan;
        IsEnableRemotePlaying = isEnableRemotePlaying;
        IsEnableRemoteRecording = isEnableRemoteRecording;
    }

    public string MediaPlayerHost { get; }
    public string RecorderHost { get; }
    public TimeSpan RecordingSpan { get; }
    public bool IsEnableRemotePlaying { get; }
    public bool IsEnableRemoteRecording { get; }
}