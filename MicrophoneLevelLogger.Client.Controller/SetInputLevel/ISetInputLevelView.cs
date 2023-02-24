namespace MicrophoneLevelLogger.Client.Controller.SetInputLevel;

/// <summary>
/// マイクの入力レベル設定ビュー。
/// </summary>
public interface ISetInputLevelView : IMicrophoneView
{
    /// <summary>
    /// マイクを選択する。
    /// </summary>
    /// <param name="audioInterface"></param>
    /// <returns></returns>
    IMicrophone SelectMicrophone(IAudioInterface audioInterface);
    /// <summary>
    /// 入力レベルを入力する。
    /// </summary>
    /// <returns></returns>
    float InputInputLevel();
}