namespace MicrophoneLevelLogger.Client.Controller.Record;

public interface IRecordView : IMicrophoneView
{
    public string InputRecordName();
    public void NotifyStarting(TimeSpan timeSpan);
    void Wait(TimeSpan timeout);
    public void NotifyResult(IEnumerable<RecordResult> results);
}