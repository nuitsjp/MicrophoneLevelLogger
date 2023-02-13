namespace MicrophoneLevelLogger;

public interface IRecorder
{
    Task RecodeAsync(string name);
    Task StopAsync();
}