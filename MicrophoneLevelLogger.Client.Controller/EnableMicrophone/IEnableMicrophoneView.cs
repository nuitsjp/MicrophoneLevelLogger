namespace MicrophoneLevelLogger.Client.Controller.EnableMicrophone;

public interface IEnableMicrophoneView : IMicrophoneView
{
    bool TrySelectMicrophone(IAudioInterface audioInterface, Settings settings, out IMicrophone microphone);
}