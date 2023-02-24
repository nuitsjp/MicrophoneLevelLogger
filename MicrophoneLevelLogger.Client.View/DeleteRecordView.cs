using MicrophoneLevelLogger.Client.Controller.DeleteRecord;
using Sharprompt;

namespace MicrophoneLevelLogger.Client.View;

/// <summary>
/// 録音削除ビュー
/// </summary>
public class DeleteRecordView : IDeleteRecordView
{
    /// <summary>
    /// 削除するかどうか確認する。
    /// </summary>
    /// <returns></returns>
    public bool Confirm()
    {
        return Prompt.Confirm("録音データをすべて削除しますか？", false);
    }
}