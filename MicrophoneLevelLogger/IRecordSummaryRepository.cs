namespace MicrophoneLevelLogger;

public interface IRecordSummaryRepository
{
    Task SaveAsync(RecordSummary recordSummary, DirectoryInfo directory);
    Task<IEnumerable<RecordSummary>> LoadAsync();
}