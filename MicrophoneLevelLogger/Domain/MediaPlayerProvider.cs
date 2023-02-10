namespace MicrophoneLevelLogger.Domain;

public class MediaPlayerProvider : IMediaPlayerProvider
{
    public IMediaPlayer ResolveLocaleService()
    {
        return new LocalMediaPlayer();
    }

    public IMediaPlayer ResolveRemoteService()
    {
        return new RemoteMediaPlayer();
    }
}