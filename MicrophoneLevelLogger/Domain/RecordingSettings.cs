using MicrophoneLevelLogger.Command;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace MicrophoneLevelLogger.Domain;

public class RecordingSettings
{
    private const string FileName = $"{nameof(RecordingSettings)}.json";

    public RecordingSettings(
        string mediaPlayerHost, 
        string recorderHost, 
        TimeSpan recordingSpan, 
        bool isEnableRemotePlaying, 
        bool isEnableRemoteRecording)
    {
        MediaPlayerHost = mediaPlayerHost;
        RecorderHost = recorderHost;
        RecordingSpan = recordingSpan;
        IsEnableRemotePlaying = isEnableRemotePlaying;
        IsEnableRemoteRecording = isEnableRemoteRecording;
    }

    private static JsonSerializerOptions Options => new()
    {
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public string MediaPlayerHost { get; }
    public string RecorderHost { get; }
    public TimeSpan RecordingSpan { get; }
    public bool IsEnableRemotePlaying { get; }
    public bool IsEnableRemoteRecording { get; }

    public static async Task<RecordingSettings> LoadAsync()
    {
        if (File.Exists(FileName))
        {
            await using var stream = new FileStream(FileName, FileMode.Open, FileAccess.Read);
            return (await JsonSerializer.DeserializeAsync<RecordingSettings>(stream, Options))!;
        }
        else
        {
            return new RecordingSettings(
                "localhost", 
                "localhost",
                TimeSpan.FromSeconds(30),
                false,
                false);
        }
    }

    public static async Task SaveAsync(RecordingSettings settings)
    {
        if (File.Exists(FileName))
        {
            File.Delete(FileName);
        }
        await using var stream = new FileStream(FileName, FileMode.Create, FileAccess.Write);
        await JsonSerializer.SerializeAsync(stream, settings, Options);
    }
}