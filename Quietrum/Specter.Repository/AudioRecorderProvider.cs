using NAudio.Wave;

namespace Specter.Repository;

public class AudioRecorderProvider : IAudioRecorderProvider
{
    private readonly IAudioRecordInterface _audioRecordInterface;

    public AudioRecorderProvider(IAudioRecordInterface audioRecordInterface)
    {
        _audioRecordInterface = audioRecordInterface;
    }

    public IAudioRecorder Resolve(
        IDevice targetDevice, 
        Direction direction, 
        IEnumerable<IDevice> monitoringDevices,
        WaveFormat waveFormat)
    {
        return new AudioRecorder(
            targetDevice,
            direction,
            monitoringDevices,
            waveFormat,
            _audioRecordInterface);
    }
}