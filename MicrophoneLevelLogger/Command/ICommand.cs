namespace MicrophoneLevelLogger.Command;

public interface ICommand
{
    string Name { get; }
    Task ExecuteAsync();
}