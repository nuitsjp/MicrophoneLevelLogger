namespace MicrophoneLevelLogger.Command;

public interface ICommandInvokerView : IMicrophoneView
{
    string SelectCommand(IEnumerable<string> commands);
}