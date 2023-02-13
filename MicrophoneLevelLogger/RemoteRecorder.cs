namespace MicrophoneLevelLogger;

public class RemoteRecorder : IRecorder
{
    private readonly HttpClient _httpClient = new();

    public async Task RecodeAsync(string name)
    {
        RecordingSettings settings = await RecordingSettings.LoadAsync();
        await _httpClient.GetAsync($"http://{settings.RecorderHost}:5000/Recorder/Recode/{name}");
    }

    public async Task StopAsync()
    {
        RecordingSettings settings = await RecordingSettings.LoadAsync();
        await _httpClient.GetAsync($"http://{settings.RecorderHost}:5000/Recorder/Stop");
    }
}