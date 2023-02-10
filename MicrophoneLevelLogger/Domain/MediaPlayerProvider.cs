namespace MicrophoneLevelLogger.Domain;

public class MediaPlayerProvider : IMediaPlayerProvider
{
    public IMediaPlayer ResolveLocaleService()
    {
        return new MediaPlayer();
    }

    public IMediaPlayer ResolveRemoteService()
    {
        return new RemoteMediaPlayer();
    }
}