using CommunityToolkit.Mvvm.ComponentModel.__Internals;
using Specter.Business;

namespace Quietrum.Repository;

/// <summary>
/// Settingsのリポジトリー
/// </summary>
public class SettingsRepository : RepositoryBase<Settings>, ISettingsRepository
{
    private static readonly FileInfo FileInfo = new($"{nameof(Settings)}.json");

    /// <summary>
    /// Settingsをロードする。
    /// </summary>
    /// <returns></returns>
    public Task<Settings> LoadAsync() =>
        LoadAsync(
            FileInfo,
            () =>
                new (
                    "localhost",
                    TimeSpan.FromSeconds(30),
                    null,
                    new List<MicrophoneConfig>()));

    /// <summary>
    /// Settingsを保存する。
    /// </summary>
    /// <param name="settings"></param>
    /// <returns></returns>
    public Task SaveAsync(Settings settings) => SaveAsync(FileInfo, settings);
}