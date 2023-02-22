namespace MicrophoneLevelLogger.Client.Controller;

public interface ICommandInvokerView : IMicrophoneView
{
    bool TrySelectController(IList<IController> controllers, out IController controller);
}