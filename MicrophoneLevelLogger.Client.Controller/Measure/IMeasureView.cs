namespace MicrophoneLevelLogger.Client.Controller.Measure;

public interface IMeasureView : IMicrophoneView
{
    IMicrophone SelectMicrophone(IAudioInterface audioInterface);
    int InputSpan();
    bool ConfirmPlayMedia();
    bool ConfirmReady();
    void WaitToBeStopped(TimeSpan timeout);
    void NotifyResult(AudioInterfaceInputLevels audioInterfaceInputLevels);
}