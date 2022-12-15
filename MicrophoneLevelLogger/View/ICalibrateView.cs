using MicrophoneLevelLogger.Domain;

namespace MicrophoneLevelLogger.View;

public interface ICalibrateView : IMicrophoneView
{
    IMicrophone SelectReference(IMicrophones microphones);
    bool ConfirmInvoke();
    void NotifyCalibrated(IMicrophones microphones);
}