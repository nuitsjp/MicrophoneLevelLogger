using NAudio.Wave;
using Reactive.Bindings;

namespace Specter;

public interface IAudioRecordInterface
{
    Task ActivateAsync();
    ReadOnlyReactiveCollection<AudioRecord> AudioRecords { get; } 
    
    IAudioRecording BeginRecording(
        IDevice targetDevice, 
        RecordingMethod recordingMethod, 
        IEnumerable<IDevice> monitoringDevices,
        WaveFormat waveFormat);

    IEnumerable<Decibel> ReadInputLevels(
        AudioRecord audioRecord,
        DeviceRecord deviceRecord);

    void DeleteAudioRecord(AudioRecord audioRecord);
}