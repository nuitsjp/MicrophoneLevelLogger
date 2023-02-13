using MicrophoneLevelLogger.Domain;

namespace MicrophoneLevelLogger.Client.Command.CalibrateOutput;

public interface ICalibrateOutputView
{
    IMicrophone SelectMicrophone(IAudioInterface audioInterface);
    int InputSpan();
    double InputDecibel();
    void DisplayOutputVolume(double volume);
    void DisplayDefaultOutputLevel(VolumeLevel level);
}