using System.Text.Json;

namespace MicrophoneLevelLogger.Repository;

public class RecordingSettingsRepository : IRecordingSettingsRepository
{
    private const string FileName = $"{nameof(RecordingSettings)}.json";

    public async Task<RecordingSettings> LoadAsync()
    {
        if (File.Exists(FileName))
        {
            await using var stream = new FileStream(FileName, FileMode.Open, FileAccess.Read);
            return (await JsonSerializer.DeserializeAsync<RecordingSettings>(stream, JsonEnvironments.Options))!;
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
        await JsonSerializer.SerializeAsync(stream, settings, JsonEnvironments.Options);
    }
}