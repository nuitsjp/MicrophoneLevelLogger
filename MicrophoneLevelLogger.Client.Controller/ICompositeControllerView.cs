namespace MicrophoneLevelLogger.Client.Controller;

public interface ICompositeControllerView
{
    bool TrySelectController(IList<IController> controllers, out IController controller);
}