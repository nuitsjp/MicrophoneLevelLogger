namespace MicrophoneLevelLogger.Domain;

public interface IMediaPlayerProvider
{
    IMediaPlayer ResolveLocaleService();

    IMediaPlayer ResolveRemoteService();
}