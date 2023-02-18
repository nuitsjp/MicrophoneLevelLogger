namespace MicrophoneLevelLogger.Client.Controller.DisplayRecords;

public interface IDisplayRecordsView
{
    DisplayType SelectDisplayRecordsType();
    RecordSummary SelectRecordSummary(IEnumerable<RecordSummary> summaries);
    void Display(RecordSummary summary);
    void Display(IEnumerable<MicrophoneRecordSummary> summaries);
}