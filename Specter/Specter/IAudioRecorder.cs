using Reactive.Bindings;

namespace Specter;

public interface IAudioRecorder
{
    void Start();
    Task<AudioRecord> StopAsync();
}

public interface IAudioRecordInterface
{
    bool Activated { get; }
    Task ActivateAsync();
    ReadOnlyReactiveCollection<AudioRecord> AudioRecords { get; } 
}
