namespace MicrophoneLevelLogger.Client.Controller.CalibrateOutput;

public interface ICalibrateOutputView
{
    IMicrophone SelectMicrophone(IAudioInterface audioInterface);
    int InputSpan();
    double InputDecibel();
    void DisplayOutputVolume(double volume);
    void DisplayDefaultOutputLevel(VolumeLevel level);
}