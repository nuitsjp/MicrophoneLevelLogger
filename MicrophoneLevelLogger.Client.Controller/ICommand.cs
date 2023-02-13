namespace MicrophoneLevelLogger.Client.Command;

public interface ICommand
{
    string Name { get; }
    Task ExecuteAsync();
}