using MicrophoneLevelLogger.Domain;

namespace MicrophoneLevelLogger.Command;

public interface ISetInputLevelView : IMicrophoneView
{
    IMicrophone SelectMicrophone(IAudioInterface audioInterface);
    float InputInputLevel();
}