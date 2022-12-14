using MicrophoneLevelLogger.Domain;

namespace MicrophoneLevelLogger.View;

public interface ICalibrateView : IMicrophoneView
{
    bool ConfirmInvoke();
    void NotifyCalibrated(IMicrophones microphones);
}