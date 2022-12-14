using MicrophoneNoiseAnalyzer.Domain;

namespace MicrophoneNoiseAnalyzer.View;

public interface IMicrophoneView
{
    void NotifyMicrophonesInformation(IMicrophones microphones);
    void StartNotifyMasterPeakValue(IMicrophones microphones);
    void StopNotifyMasterPeakValue();
}