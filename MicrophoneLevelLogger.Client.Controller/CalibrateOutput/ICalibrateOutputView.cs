namespace MicrophoneLevelLogger.Client.Controller.CalibrateOutput;

public interface ICalibrateOutputView
{
    IMicrophone SelectMicrophone(IAudioInterface audioInterface);
    int InputSpan();
    Decibel InputDecibel();
    void DisplayOutputVolume(Decibel volume);
    void DisplayDefaultOutputLevel(VolumeLevel level);
}