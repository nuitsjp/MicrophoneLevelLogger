using MicrophoneLevelLogger.Domain;

namespace MicrophoneLevelLogger.View;

public interface IRecordView : IMicrophoneView
{
    public void NotifyResult(IEnumerable<IMasterPeakValues> values);
}