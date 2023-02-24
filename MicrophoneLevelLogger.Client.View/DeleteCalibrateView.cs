using MicrophoneLevelLogger.Client.Controller.DeleteCalibrates;
using Sharprompt;

namespace MicrophoneLevelLogger.Client.View;

/// <summary>
/// マイク入力レベルの調整結果削除ビュー
/// </summary>
public class DeleteCalibrateView : IDeleteCalibrateView
{
    /// <summary>
    /// 削除するかどうか確認する。
    /// </summary>
    /// <returns></returns>
    public bool Confirm()
    {
        return Prompt.Confirm("マイク入力レベルの調整結果をすべて削除しますか？", false);
    }
}