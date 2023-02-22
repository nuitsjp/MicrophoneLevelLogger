namespace MicrophoneLevelLogger.Client.Controller;

public class BorderController : IController
{
    public string Name => "-----------------------------------------------------------";
    public string Description => "-----------------------------------------------------------";
    public Task ExecuteAsync() => Task.CompletedTask;
}