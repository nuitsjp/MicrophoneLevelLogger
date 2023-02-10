using MicrophoneLevelLogger.Domain;

namespace MicrophoneLevelLogger.Command;

public interface ICalibrateView : IMicrophoneView
{
    IMicrophone SelectReference(IAudioInterface audioInterface);
    IMicrophone SelectTarget(IAudioInterface audioInterface, IMicrophone reference);
    void NotifyCalibrated(IAudioInterface audioInterface);
}