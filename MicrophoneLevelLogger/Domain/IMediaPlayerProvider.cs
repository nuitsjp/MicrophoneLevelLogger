namespace MicrophoneLevelLogger.Domain;

public interface IMediaPlayerProvider
{
    IMediaPlayer ResolveLocale();

    IMediaPlayer ResolveRemote();
}