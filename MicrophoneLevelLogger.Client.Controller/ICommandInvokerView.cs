namespace MicrophoneLevelLogger.Client.Command;

public interface ICommandInvokerView : IMicrophoneView
{
    string SelectCommand(IEnumerable<string> commands);
}