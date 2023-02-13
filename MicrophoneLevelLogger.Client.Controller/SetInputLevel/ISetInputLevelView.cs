namespace MicrophoneLevelLogger.Client.Command.SetInputLevel;

public interface ISetInputLevelView : IMicrophoneView
{
    IMicrophone SelectMicrophone(IAudioInterface audioInterface);
    float InputInputLevel();
}