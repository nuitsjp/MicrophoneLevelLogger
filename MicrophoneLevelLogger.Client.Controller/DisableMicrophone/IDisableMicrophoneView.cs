namespace MicrophoneLevelLogger.Client.Controller.DisableMicrophone;

/// <summary>
/// 指定のマイクを無効化するビュー
/// </summary>
public interface IDisableMicrophoneView : IMicrophoneView
{
    /// <summary>
    /// 無効化するマイクを選択する。
    /// </summary>
    /// <param name="audioInterface"></param>
    /// <param name="microphone"></param>
    /// <returns></returns>
    bool TrySelectMicrophone(IAudioInterface audioInterface, out IMicrophone microphone);
}