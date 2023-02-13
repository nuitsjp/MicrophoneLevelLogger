namespace MicrophoneLevelLogger;

public class RemoteMediaPlayer : IMediaPlayer
{
    private readonly HttpClient _httpClient = new();
    private readonly IRecordingSettingsRepository _repository;

    public RemoteMediaPlayer(IRecordingSettingsRepository repository)
    {
        _repository = repository;
    }

    public async Task PlayLoopingAsync()
    {
        RecordingSettings settings = await _repository.LoadAsync();
        await _httpClient.GetAsync($"http://{settings.MediaPlayerHost}:5000/Player/Play");
    }

    public async Task StopAsync()
    {
        RecordingSettings settings = await _repository.LoadAsync();
        await _httpClient.GetAsync($"http://{settings.MediaPlayerHost}:5000/Player/Stop");
    }
}