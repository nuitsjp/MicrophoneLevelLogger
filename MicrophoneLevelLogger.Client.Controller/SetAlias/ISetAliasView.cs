namespace MicrophoneLevelLogger.Client.Controller.SetAlias;

/// <summary>
/// マイクに別名を設定する。
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