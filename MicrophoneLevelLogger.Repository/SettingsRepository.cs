using System.Text.Json;

namespace MicrophoneLevelLogger.Repository;

public class SettingsRepository : ISettingsRepository
{
    private const string FileName = $"{nameof(Settings)}.json";

    public async Task<Settings> LoadAsync()
    {
        if (File.Exists(FileName))
        {
            await using var stream = new FileStream(FileName, FileMode.Open, FileAccess.Read);
            return (await JsonSerializer.DeserializeAsync<Settings>(stream, JsonEnvironments.Options))!;
        }
        else
        {
            return new Settings(
                "localhost",
                "localhost",
                TimeSpan.FromSeconds(30),
                false,
                false,
                new List<Alias>(),
                new List<MicrophoneId>());
        }
    }

    public Settings Load()
    {
        if (File.Exists(FileName))
        {
            using var stream = new FileStream(FileName, FileMode.Open, FileAccess.Read);
            return JsonSerializer.Deserialize<Settings>(stream, JsonEnvironments.Options)!;
        }
        else
        {
            return new Settings(
                "localhost",
                "localhost",
                TimeSpan.FromSeconds(30),
                false,
                false,
                new List<Alias>(),
                new List<MicrophoneId>());
        }
    }

    public async Task SaveAsync(Settings settings)
    {
        if (File.Exists(FileName))
        {
            File.Delete(FileName);
        }
        await using var stream = new FileStream(FileName, FileMode.Create, FileAccess.Write);
        await JsonSerializer.SerializeAsync(stream, settings, JsonEnvironments.Options);
    }
}