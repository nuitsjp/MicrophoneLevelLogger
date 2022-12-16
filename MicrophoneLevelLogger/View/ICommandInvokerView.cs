using MicrophoneLevelLogger.Domain;

namespace MicrophoneLevelLogger.View;

public interface ICommandInvokerView : IMicrophoneView
{
    string SelectCommand(IEnumerable<string> commands);
}