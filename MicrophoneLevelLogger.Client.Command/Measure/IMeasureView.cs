namespace MicrophoneLevelLogger.Client.Command.Measure;

public interface IMeasureView : IMicrophoneView
{
    IMicrophone SelectMicrophone(IAudioInterface audioInterface);
    int InputSpan();
    bool ConfirmPlayMedia();
    bool ConfirmReady();
    void NotifyResult(AudioInterfaceInputLevels audioInterfaceInputLevels);
}