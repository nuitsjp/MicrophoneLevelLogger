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
        BuzzState buzzState,
        IEnumerable<IDevice> monitoringDevices,
        IRenderDevice? playbackDevice,
        WaveFormat waveFormat);

    IEnumerable<Decibel> ReadInputLevels(
        AudioRecord audioRecord,
        DeviceRecord deviceRecord);

    void DeleteAudioRecord(AudioRecord audioRecord);
}