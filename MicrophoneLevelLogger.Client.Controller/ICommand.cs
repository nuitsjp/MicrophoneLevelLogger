namespace MicrophoneLevelLogger.Client.Controller;

public interface ICommand
{
    string Name { get; }
    Task ExecuteAsync();
}