namespace MicrophoneLevelLogger.Client.Controller;

public interface ICommandInvokerView : IMicrophoneView
{
    string SelectCommand(IEnumerable<string> commands);
}