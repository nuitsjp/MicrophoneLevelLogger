namespace MicrophoneLevelLogger;

public interface IMediaPlayer
{
    Task PlayLoopingAsync(CancellationToken token);
}