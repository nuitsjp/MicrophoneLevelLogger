using System.Text.Json;

namespace Quietrum.Repository;

/// <summary>
/// Settingsのリポジトリー
/// </summary>
public class SettingsRepository : ISettingsRepository
{
    private const string FileName = $"{nameof(Settings)}.json";

    /// <summary>
    /// 外部から設定ファイルを更新されることは想定しないため、インスタンスをキャッシュする。
    /// 主にファイルの同時アクセス制御を簡素化することが目的。
    /// </summary>
    private static Settings? _settings;

    /// <summary>
    /// Settingsをロードする。
    /// </summary>
    /// <returns></returns>
    public async Task<Settings> LoadAsync()
    {
        if (_settings is not null) return _settings;
        
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
        _settings = (await JsonSerializer.DeserializeAsync<Settings>(stream, JsonEnvironments.Options))!;
        return _settings;
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

        _settings = settings;
        await using var stream = new FileStream(FileName, FileMode.Create, FileAccess.Write);
        await JsonSerializer.SerializeAsync(stream, settings, JsonEnvironments.Options);
    }
}