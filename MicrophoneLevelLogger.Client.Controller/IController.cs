namespace MicrophoneLevelLogger.Client.Controller;

public interface IController
{
    string Name { get; }
    Task ExecuteAsync();
}