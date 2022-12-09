using MicrophoneNoiseAnalyzer.Domain;

namespace MicrophoneNoiseAnalyzer.View;

public interface ICalibrationView
{
    void NotifyMicrophonesInformation(IMicrophones microphones);
    bool ConfirmInvoke();
    void NotifyMasterPeakValue(IMicrophones microphones);
}