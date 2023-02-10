using MicrophoneLevelLogger.Domain;

namespace MicrophoneLevelLogger.Command;

public interface IRecordView : IMicrophoneView
{
    public void NotifyResult(IEnumerable<RecordResult> results);
    public void NotifyStarting(TimeSpan timeSpan);
}