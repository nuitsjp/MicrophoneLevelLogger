using NAudio.Wave;

namespace Specter.Repository;

public class AudioRecorderProvider : IAudioRecorderProvider
{
    private readonly IAudioRecordRepository _audioRecordRepository;

    public AudioRecorderProvider(IAudioRecordRepository audioRecordRepository)
    {
        _audioRecordRepository = audioRecordRepository;
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
            _audioRecordRepository);
    }
}