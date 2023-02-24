namespace MicrophoneLevelLogger.Client.Controller.SetAlias;

/// <summary>
/// マイクの別名設定ビュー。
/// </summary>
public interface ISetAliasView
{
    /// <summary>
    /// マイクを選択する。
    /// </summary>
    /// <param name="audioInterface"></param>
    /// <returns></returns>
    IMicrophone SelectMicrophone(IAudioInterface audioInterface);
    /// <summary>
    /// 別名を入力する。
    /// </summary>
    /// <param name="microphone"></param>
    /// <returns></returns>
    string InputAlias(IMicrophone microphone);
}