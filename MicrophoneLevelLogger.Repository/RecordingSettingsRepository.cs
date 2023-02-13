using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace MicrophoneLevelLogger.Repository;

public class RecordingSettingsRepository : IRecordingSettingsRepository
{
    private const string FileName = $"{nameof(RecordingSettings)}.json";

    private static JsonSerializerOptions Options => new()
    {
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public async Task<RecordingSettings> LoadAsync()
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

    public async Task SaveAsync(RecordingSettings settings)
    {
        if (File.Exists(FileName))
        {
            File.Delete(FileName);
        }
        await using var stream = new FileStream(FileName, FileMode.Create, FileAccess.Write);
        await JsonSerializer.SerializeAsync(stream, settings, Options);
    }
}