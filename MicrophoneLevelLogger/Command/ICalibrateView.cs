using MicrophoneLevelLogger.Domain;

namespace MicrophoneLevelLogger.Command;

public interface ICalibrateView : IMicrophoneView
{
    IMicrophone SelectReference(IMicrophones microphones);
    void NotifyCalibrated(IMicrophones microphones);
}