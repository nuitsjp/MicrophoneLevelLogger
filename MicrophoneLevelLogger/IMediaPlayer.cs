namespace MicrophoneLevelLogger;

public interface IMediaPlayer : IDisposable
{
    Task PlayLoopingAsync(CancellationToken token);
}