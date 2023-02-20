namespace MicrophoneLevelLogger.Client.Controller.ExcludeMicrophone;

public interface IExcludeMicrophoneView
{
    bool TrySelectMicrophone(IAudioInterface audioInterface, out IMicrophone microphone);
}