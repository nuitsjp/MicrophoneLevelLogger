using MicrophoneLevelLogger.Domain;

namespace MicrophoneLevelLogger.Command.SetInputLevel;

public interface ISetInputLevelView : IMicrophoneView
{
    IMicrophone SelectMicrophone(IAudioInterface audioInterface);
    float InputInputLevel();
}