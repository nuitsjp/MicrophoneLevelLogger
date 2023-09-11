using NAudio.Wave;
using Reactive.Bindings;

namespace Specter;

public interface IAudioRecordInterface
{
    bool Activated { get; }
    Task ActivateAsync();
    ReadOnlyReactiveCollection<AudioRecord> AudioRecords { get; } 
    
    IAudioRecording BeginRecording(
        IDevice targetDevice, 
        Direction direction, 
        IEnumerable<IDevice> monitoringDevices,
        WaveFormat waveFormat);
}