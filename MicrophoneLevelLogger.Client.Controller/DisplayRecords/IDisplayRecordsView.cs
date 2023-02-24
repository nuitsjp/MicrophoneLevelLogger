namespace MicrophoneLevelLogger.Client.Controller.DisplayRecords;

/// <summary>
/// 記録表示ビュー
/// </summary>
public interface IDisplayRecordsView
{
    /// <summary>
    /// 表示内容を選択する。
    /// </summary>
    /// <returns></returns>
    DisplayType SelectDisplayRecordsType();
    /// <summary>
    /// 表示する記録を選択する。
    /// </summary>
    /// <param name="summaries"></param>
    /// <returns></returns>
    RecordSummary SelectRecordSummary(IEnumerable<RecordSummary> summaries);
    /// <summary>
    /// 記録を表示する。
    /// </summary>
    /// <param name="summary"></param>
    void Display(RecordSummary summary);
    /// <summary>
    /// マイク別の記録を表示する。
    /// </summary>
    /// <param name="summaries"></param>
    void Display(IEnumerable<MicrophoneRecordSummary> summaries);
}