namespace MicrophoneLevelLogger.Client.Controller.DisplayCalibrates;

/// <summary>
/// マイク調整結果ビュー
/// </summary>
public interface IDisplayCalibratesView
{
    /// <summary>
    /// 調整結果を表示する。
    /// </summary>
    /// <param name="calibrates"></param>
    void NotifyResult(AudioInterfaceCalibrationValues calibrates);
}