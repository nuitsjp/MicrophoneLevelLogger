namespace MicrophoneLevelLogger.Domain;

public interface IRecorder
{
    Task RecodeAsync();
    Task StopAsync();
}