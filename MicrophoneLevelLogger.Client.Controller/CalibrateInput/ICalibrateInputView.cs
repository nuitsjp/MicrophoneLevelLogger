namespace MicrophoneLevelLogger.Client.Controller.CalibrateInput;

/// <summary>
/// マイク調整用ビュー
/// </summary>
public interface ICalibrateInputView : IMicrophoneView
{
    /// <summary>
    /// リファレンスマイクを選択する。
    /// </summary>
    /// <param name="audioInterface"></param>
    /// <returns></returns>
    IMicrophone SelectReference(IAudioInterface audioInterface);

    /// <summary>
    /// 調整対象マイクを選択する。
    /// </summary>
    /// <param name="audioInterface"></param>
    /// <param name="reference"></param>
    /// <returns></returns>
    IMicrophone SelectTarget(IAudioInterface audioInterface, IMicrophone reference);

    /// <summary>
    /// 調整の進捗を表示する。
    /// </summary>
    /// <param name="reference"></param>
    /// <param name="referenceDecibel"></param>
    /// <param name="target"></param>
    /// <param name="targetDecibel"></param>
    void NotifyProgress(IMicrophone reference, Decibel referenceDecibel, IMicrophone target, Decibel targetDecibel);

    /// <summary>
    /// 調整結果を通知する。
    /// </summary>
    /// <param name="calibrationValue"></param>
    /// <param name="microphone"></param>
    void NotifyCalibrated(AudioInterfaceCalibrationValues calibrationValue, IMicrophone microphone);
}