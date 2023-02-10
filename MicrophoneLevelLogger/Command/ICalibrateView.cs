using MicrophoneLevelLogger.Domain;

namespace MicrophoneLevelLogger.Command;

public interface ICalibrateView : IMicrophoneView
{
    IMicrophone SelectReference(IAudioInterface audioInterface);
    IMicrophone SelectTarget(IAudioInterface audioInterface, IMicrophone reference);
    void NotifyProgress(IMicrophone reference, double referenceDecibel, IMicrophone target, double targetDecibel);
    void NotifyCalibrated(AudioInterfaceCalibrationValues calibrationValue, IMicrophone microphone);
}