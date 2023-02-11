namespace MicrophoneLevelLogger.Domain;

public interface IRecorder
{
    Task RecodeAsync(string name);
    Task StopAsync();
}