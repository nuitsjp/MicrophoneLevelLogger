namespace MicrophoneLevelLogger.Client.Controller.DeleteRecord;

/// <summary>
/// 録音削除ビュー
/// </summary>
public interface IDeleteRecordView
{
    /// <summary>
    /// 削除するかどうか確認する。
    /// </summary>
    /// <returns></returns>
    bool Confirm();
}