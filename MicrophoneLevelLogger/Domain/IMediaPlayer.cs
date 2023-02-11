namespace MicrophoneLevelLogger.Domain;

public interface IMediaPlayer
{
    Task PlayLoopingAsync();
    Task StopAsync();
}