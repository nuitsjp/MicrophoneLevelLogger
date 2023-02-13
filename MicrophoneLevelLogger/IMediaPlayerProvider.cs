namespace MicrophoneLevelLogger;

public interface IMediaPlayerProvider
{
    IMediaPlayer ResolveLocale();

    IMediaPlayer ResolveRemote();
}