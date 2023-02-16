namespace MicrophoneLevelLogger;

public class MediaPlayerProvider : IMediaPlayerProvider
{
    private readonly IRecordingSettingsRepository _repository;

    public MediaPlayerProvider(IRecordingSettingsRepository repository)
    {
        _repository = repository;
    }

    public IMediaPlayer Resolve(bool isRemotePlay) =>
        isRemotePlay
            ? new RemoteMediaPlayer(_repository)
            : new MediaPlayer();

}