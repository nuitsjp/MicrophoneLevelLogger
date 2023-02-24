namespace MicrophoneLevelLogger.Client.Controller.CalibrateOutput;

/// <summary>
/// スピーカー調整ビュー
/// </summary>
public interface ICalibrateOutputView : IMicrophoneView
{
    /// <summary>
    /// 調整に利用するマイクを選択する。
    /// </summary>
    /// <param name="audioInterface"></param>
    /// <returns></returns>
    IMicrophone SelectMicrophone(IAudioInterface audioInterface);
    /// <summary>
    /// 調整に利用する録音時間を入力する。
    /// </summary>
    /// <returns></returns>
    int InputSpan();
    /// <summary>
    /// スピーカーの調整先の音量を入力する。
    /// </summary>
    /// <returns></returns>
    Decibel InputDecibel();
    /// <summary>
    /// 計測結果のボリュームを表示する。
    /// </summary>
    /// <param name="volume"></param>
    void DisplayOutputVolume(Decibel volume);
    /// <summary>
    /// スピーカーの音量レベルを表示する。
    /// </summary>
    /// <param name="level"></param>
    void DisplaySpeakerVolumeLevel(VolumeLevel level);
}