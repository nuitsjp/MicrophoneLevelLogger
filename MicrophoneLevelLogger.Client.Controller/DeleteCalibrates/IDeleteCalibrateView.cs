namespace MicrophoneLevelLogger.Client.Controller.DeleteCalibrates;

/// <summary>
/// マイク入力レベルの調整結果削除ビュー
/// </summary>
public interface IDeleteCalibrateView
{
    /// <summary>
    /// 削除するかどうか確認する。
    /// </summary>
    /// <returns></returns>
    bool Confirm();
}