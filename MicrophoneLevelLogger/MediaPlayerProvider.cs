namespace MicrophoneLevelLogger;

public class MediaPlayerProvider : IMediaPlayerProvider
{
    private readonly ISettingsRepository _repository;

    public MediaPlayerProvider(ISettingsRepository repository)
    {
        _repository = repository;
    }

    public IMediaPlayer Resolve(bool isRemotePlay) =>
        isRemotePlay
            ? new RemoteMediaPlayer(_repository)
            : new MediaPlayer();

}