namespace MicrophoneLevelLogger;

public interface IMicrophoneLogger : IDisposable
{
    public IMicrophone Microphone { get; }
    public Decibel Max { get; }
    public Decibel Avg { get; }
    public Decibel Min { get; }
    public Task StartAsync(CancellationToken token);
}