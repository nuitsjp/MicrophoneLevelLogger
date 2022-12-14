using MicrophoneLevelLogger.Domain;

namespace MicrophoneLevelLogger.View;

public interface IMicrophoneView
{
    void NotifyMicrophonesInformation(IMicrophones microphones);
    void StartNotifyMasterPeakValue(IMicrophones microphones);
    void StopNotifyMasterPeakValue();
}