using MicrophoneLevelLogger.Domain;

namespace MicrophoneLevelLogger.View;

public interface ICalibrateView : IMicrophoneView
{
    IMicrophone SelectReference(IMicrophones microphones);
    void NotifyCalibrated(IMicrophones microphones);
}