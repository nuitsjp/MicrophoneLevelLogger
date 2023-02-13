namespace MicrophoneLevelLogger;

public class MediaPlayerProvider : IMediaPlayerProvider
{
    private readonly IRecordingSettingsRepository _repository;

    public MediaPlayerProvider(IRecordingSettingsRepository repository)
    {
        _repository = repository;
    }

    public IMediaPlayer ResolveLocale()
    {
        return new MediaPlayer();
    }

    public IMediaPlayer ResolveRemote()
    {
        return new RemoteMediaPlayer(_repository);
    }
}