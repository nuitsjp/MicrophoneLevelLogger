namespace MicrophoneLevelLogger;

public interface IMediaPlayerProvider
{
    IMediaPlayer Resolve(bool isRemotePlay);
}