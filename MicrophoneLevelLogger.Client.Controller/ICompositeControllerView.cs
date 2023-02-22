namespace MicrophoneLevelLogger.Client.Controller;

public interface ICompositeControllerView
{
    bool TrySelectController(CompositeController composite, out IController controller);
}