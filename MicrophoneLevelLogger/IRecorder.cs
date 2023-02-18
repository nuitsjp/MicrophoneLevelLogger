namespace MicrophoneLevelLogger;

public interface IRecorder : IDisposable
{
    IReadOnlyList<IMicrophoneRecorder> MicrophoneRecorders { get; }
    public Task StartAsync(CancellationToken token);
    public IMicrophoneRecorder GetLogger(IMicrophone microphone);
}