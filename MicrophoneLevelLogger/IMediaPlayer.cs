namespace MicrophoneLevelLogger;

public interface IMediaPlayer
{
    Task PlayLoopingAsync();
    Task StopAsync();
}