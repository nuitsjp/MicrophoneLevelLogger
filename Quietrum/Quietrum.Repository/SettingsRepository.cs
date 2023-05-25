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
        if (File.Exists(FileName) is false)
        {
            await SaveAsync(new Settings(
                "localhost",
                "localhost",
                TimeSpan.FromSeconds(30),
                false,
                false,
                null,
                new List<MicrophoneConfig>()));
        }

        await using var stream = new FileStream(FileName, FileMode.Open, FileAccess.Read);
        return (await JsonSerializer.DeserializeAsync<Settings>(stream, JsonEnvironments.Options))!;
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