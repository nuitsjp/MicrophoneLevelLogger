using MicrophoneLevelLogger.Domain;

namespace MicrophoneLevelLogger.Command;

public interface IShowInputLevelView
{
    void NotifyResult(AudioInterfaceInputLevels audioInterfaceInputLevels);
}