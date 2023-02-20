namespace MicrophoneLevelLogger.Client.Controller.ExcludeMicrophone;

public interface IDisableMicrophoneView
{
    bool TrySelectMicrophone(IAudioInterface audioInterface, out IMicrophone microphone);
}