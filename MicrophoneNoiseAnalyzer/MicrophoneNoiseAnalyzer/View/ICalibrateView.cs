using MicrophoneNoiseAnalyzer.Domain;

namespace MicrophoneNoiseAnalyzer.View;

public interface ICalibrateView : IMicrophoneView
{
    bool ConfirmInvoke();
    void NotifyCalibrated(IMicrophones microphones);
}