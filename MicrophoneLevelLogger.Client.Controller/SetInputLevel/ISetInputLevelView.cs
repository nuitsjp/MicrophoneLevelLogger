namespace MicrophoneLevelLogger.Client.Controller.SetInputLevel;

public interface ISetInputLevelView : IMicrophoneView
{
    IMicrophone SelectMicrophone(IAudioInterface audioInterface);
    float InputInputLevel();
}