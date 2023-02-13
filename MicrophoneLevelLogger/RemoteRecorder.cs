namespace MicrophoneLevelLogger;

public class RemoteRecorder : IRecorder
{
    private readonly HttpClient _httpClient = new();

    private readonly IRecordingSettingsRepository _repository;

    public RemoteRecorder(IRecordingSettingsRepository repository)
    {
        _repository = repository;
    }

    public async Task RecodeAsync(string name)
    {
        RecordingSettings settings = await _repository.LoadAsync();
        await _httpClient.GetAsync($"http://{settings.RecorderHost}:5000/Recorder/Recode/{name}");
    }

    public async Task StopAsync()
    {
        RecordingSettings settings = await _repository.LoadAsync();
        await _httpClient.GetAsync($"http://{settings.RecorderHost}:5000/Recorder/Stop");
    }
}