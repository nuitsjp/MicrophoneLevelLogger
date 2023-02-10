using System.Net.Http;

namespace MicrophoneLevelLogger.Domain;

public class RemoteMediaPlayer : IMediaPlayer
{
    private readonly HttpClient _httpClient = new();

    public async Task PlayAsync()
    {
        RecordingSettings settings = await RecordingSettings.LoadAsync();
        await _httpClient.GetAsync($"http://{settings.MediaPlayerHost}:5000/Player/Play");
    }

    public async Task StopAsync()
    {
        RecordingSettings settings = await RecordingSettings.LoadAsync();
        await _httpClient.GetAsync($"http://{settings.MediaPlayerHost}:5000/Player/Stop");
    }
}