using MicrophoneLevelLogger.Domain;

namespace MicrophoneLevelLogger.Command;

public interface IMeasureInputLevelView : IMicrophoneView
{
    IMicrophone SelectMicrophone(IAudioInterface audioInterface);
    void NotifyResult(AudioInterfaceInputLevels audioInterfaceInputLevels);
}