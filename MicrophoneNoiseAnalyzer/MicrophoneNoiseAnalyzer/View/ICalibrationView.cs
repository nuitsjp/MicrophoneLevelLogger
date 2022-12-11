using MicrophoneNoiseAnalyzer.Domain;

namespace MicrophoneNoiseAnalyzer.View;

public interface ICalibrationView
{
    void NotifyMicrophonesInformation(IMicrophones microphones);
    bool ConfirmInvoke();
    void StartNotifyMasterPeakValue(IMicrophones microphones);
    void StopNotifyMasterPeakValue();
    void NotifyCalibrated(IMicrophones microphones);
}