using MicrophoneLevelLogger.Domain;

namespace MicrophoneLevelLogger.Command;

public interface IRemoveInputLevelsView
{
    bool Confirm();
}