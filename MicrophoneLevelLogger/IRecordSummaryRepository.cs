namespace MicrophoneLevelLogger;

public interface IRecordSummaryRepository
{
    Task SaveAsync(RecordSummary recordSummary, DirectoryInfo directory);
    IAsyncEnumerable<RecordSummary> LoadAsync();
}