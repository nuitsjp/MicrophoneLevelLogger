namespace MicrophoneLevelLogger.Domain;

public class RemoteRecorder : IRecorder
{
    private readonly HttpClient _httpClient = new();

    public async Task RecodeAsync()
    {
        RecordingSettings settings = await RecordingSettings.LoadAsync();
        await _httpClient.GetAsync($"http://{settings.RecorderHost}:5000/Recorder/Recode");
    }

    public async Task StopAsync()
    {
        RecordingSettings settings = await RecordingSettings.LoadAsync();
        await _httpClient.GetAsync($"http://{settings.RecorderHost}:5000/Recorder/Stop");
    }
}