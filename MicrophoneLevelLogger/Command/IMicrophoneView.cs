using MicrophoneLevelLogger.Domain;

namespace MicrophoneLevelLogger.Command;

public interface IMicrophoneView
{
    void NotifyMicrophonesInformation(IMicrophones microphones);
    void StartNotifyMasterPeakValue(IMicrophones microphones);
    void StopNotifyMasterPeakValue();
}