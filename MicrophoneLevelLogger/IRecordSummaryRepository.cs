namespace MicrophoneLevelLogger;

/// <summary>
/// RecordSummaryのリポジトリー
/// </summary>
public interface IRecordSummaryRepository
{
    /// <summary>
    /// RecordSummaryを保存する。
    /// </summary>
    /// <param name="recordSummary"></param>
    /// <param name="directory"></param>
    /// <returns></returns>
    Task SaveAsync(RecordSummary recordSummary, DirectoryInfo directory);
    /// <summary>
    /// RecordSummaryをロードする。
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<RecordSummary>> LoadAsync();
}