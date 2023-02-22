namespace MicrophoneLevelLogger.Client.Controller;

public interface IController
{
    string Name { get; }
    string Description { get; }
    Task ExecuteAsync();
}