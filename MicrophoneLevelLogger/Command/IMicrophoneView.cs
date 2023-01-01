using MicrophoneLevelLogger.Domain;

namespace MicrophoneLevelLogger.Command;

public interface IMicrophoneView
{
    void NotifyMicrophonesInformation(IAudioInterface audioInterface);
    void StartNotifyMasterPeakValue(IAudioInterface audioInterface);
    void StopNotifyMasterPeakValue();
}