namespace MicrophoneLevelLogger.Client.Controller;

public interface ICompositeControllerView
{
    IController SelectController(IList<IController> controllers);
}