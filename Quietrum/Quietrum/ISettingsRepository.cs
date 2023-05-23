namespace Quietrum;

/// <summary>
/// Settingsのリポジトリー
/// </summary>
public interface ISettingsRepository
{
    /// <summary>
    /// Settingsをロードする。
    /// </summary>
    /// <returns></returns>
    Task<Settings> LoadAsync();
    /// <summary>
    /// Settingsをロードする。
    /// </summary>
    /// <returns></returns>
    Settings Load();
    /// <summary>
    /// Settingsを保存する。
    /// </summary>
    /// <param name="settings"></param>
    /// <returns></returns>
    Task SaveAsync(Settings settings);
}