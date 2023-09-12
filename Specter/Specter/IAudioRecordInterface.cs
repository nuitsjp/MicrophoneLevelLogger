using NAudio.Wave;
using Reactive.Bindings;

namespace Specter;

public interface IAudioRecordInterface
{
    Task ActivateAsync();
    ReadOnlyReactiveCollection<AudioRecord> AudioRecords { get; } 
    
    IAudioRecording BeginRecording(
        IDevice targetDevice, 
        Direction direction, 
        IEnumerable<IDevice> monitoringDevices,
        WaveFormat waveFormat);

    IEnumerable<Decibel> ReadInputLevels(
        AudioRecord audioRecord,
        DeviceRecord deviceRecord);
}