using MicrophoneLevelLogger.Domain;

namespace MicrophoneLevelLogger.Command;

public interface ICalibrateView : IMicrophoneView
{
    IMicrophone SelectReference(IAudioInterface audioInterface);
    void NotifyCalibrated(IAudioInterface audioInterface);
}