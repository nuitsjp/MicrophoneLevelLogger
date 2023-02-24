namespace MicrophoneLevelLogger.Client.Controller.RemoveAlias;

/// <summary>
/// 別名削除ビュー
/// </summary>
public interface IRemoveAliasView
{
    /// <summary>
    /// 削除する別名を選択する。
    /// </summary>
    /// <param name="settings"></param>
    /// <param name="alias"></param>
    /// <returns></returns>
    bool TrySelectRemoveAlias(Settings settings, out Alias alias);
}