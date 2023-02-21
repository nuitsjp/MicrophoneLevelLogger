namespace MicrophoneLevelLogger.Client.Controller.DisableMicrophone;

public interface IDisableMicrophoneView : IMicrophoneView
{
    bool TrySelectMicrophone(IAudioInterface audioInterface, out IMicrophone microphone);
}