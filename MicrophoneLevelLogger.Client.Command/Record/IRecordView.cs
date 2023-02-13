using MicrophoneLevelLogger.Domain;

namespace MicrophoneLevelLogger.Client.Command.Record;

public interface IRecordView : IMicrophoneView
{
    public string InputRecordName();
    public void NotifyResult(IEnumerable<RecordResult> results);
    public void NotifyStarting(TimeSpan timeSpan);
}