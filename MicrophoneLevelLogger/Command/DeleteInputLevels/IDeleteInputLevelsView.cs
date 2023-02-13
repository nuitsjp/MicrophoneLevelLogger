using MicrophoneLevelLogger.Domain;

namespace MicrophoneLevelLogger.Command.DeleteInputLevels;

public interface IDeleteInputLevelsView
{
    bool Confirm();
}