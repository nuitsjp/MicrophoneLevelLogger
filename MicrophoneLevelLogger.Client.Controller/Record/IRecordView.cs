namespace MicrophoneLevelLogger.Client.Controller.Record;

public interface IRecordView : IMicrophoneView
{
    public string InputRecordName();
    public void NotifyResult(IEnumerable<RecordResult> results);
    public void NotifyStarting(TimeSpan timeSpan);
}