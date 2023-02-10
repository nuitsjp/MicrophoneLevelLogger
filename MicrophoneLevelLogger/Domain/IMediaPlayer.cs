namespace MicrophoneLevelLogger.Domain;

public interface IMediaPlayer
{
    Task PlayAsync();
    Task StopAsync();
}