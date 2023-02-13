namespace MicrophoneLevelLogger.Domain;

public class MediaPlayerProvider : IMediaPlayerProvider
{
    public IMediaPlayer ResolveLocale()
    {
        return new MediaPlayer();
    }

    public IMediaPlayer ResolveRemote()
    {
        return new RemoteMediaPlayer();
    }
}