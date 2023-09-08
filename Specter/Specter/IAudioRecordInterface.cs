using Reactive.Bindings;

namespace Specter;

public interface IAudioRecordInterface
{
    bool Activated { get; }
    Task ActivateAsync();
    ReadOnlyReactiveCollection<AudioRecord> AudioRecords { get; } 
    
    Task SaveAsync(AudioRecord audioRecord);
    Task<IEnumerable<AudioRecord>> LoadAsync();
}