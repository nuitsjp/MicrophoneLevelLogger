using System.Text.Json;

namespace Quietrum.Repository;

/// <summary>
/// Settingsのリポジトリー
/// </summary>
public class SettingsRepository : ISettingsRepository
{
    private const string FileName = $"{nameof(Settings)}.json";

    /// <summary>
    /// Settingsをロードする。
    /// </summary>
    /// <returns></returns>
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
                new List<MicrophoneId>(),
                null);
        }
    }

    /// <summary>
    /// Settingsをロードする。
    /// </summary>
    /// <returns></returns>
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
                new List<MicrophoneId>(),
                null);
        }
    }

    /// <summary>
    /// Settingsを保存する。
    /// </summary>
    /// <param name="settings"></param>
    /// <returns></returns>
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