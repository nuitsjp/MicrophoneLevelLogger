using NAudio.Wave;

namespace Specter;

public interface IAudioRecorderProvider
{
    IAudioRecorder Resolve(
        IDevice targetDevice, 
        Direction direction, 
        IEnumerable<IDevice> monitoringDevices,
        WaveFormat waveFormat);
}