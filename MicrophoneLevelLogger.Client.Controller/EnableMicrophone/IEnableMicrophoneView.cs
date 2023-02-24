namespace MicrophoneLevelLogger.Client.Controller.EnableMicrophone;

/// <summary>
/// マイク有効化ビュー
/// </summary>
public interface IEnableMicrophoneView : IMicrophoneView
{
    /// <summary>
    /// 有効化するマイクを選択する
    /// </summary>
    /// <param name="audioInterface"></param>
    /// <param name="settings"></param>
    /// <param name="microphone"></param>
    /// <returns></returns>
    bool TrySelectMicrophone(IAudioInterface audioInterface, Settings settings, out IMicrophone microphone);
}