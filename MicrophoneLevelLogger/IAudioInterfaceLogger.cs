namespace MicrophoneLevelLogger;

public interface IAudioInterfaceLogger : IDisposable
{
    IReadOnlyList<IMicrophoneLogger> MicrophoneLoggers { get; }
    public Task StartAsync(CancellationToken token);
    public IMicrophoneLogger GetLogger(IMicrophone microphone);
}