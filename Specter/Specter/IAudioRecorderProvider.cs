using NAudio.Wave;

namespace Specter.Business;

public interface IAudioRecorderProvider
{
    IAudioRecorder Resolve(
        IDevice targetDevice, 
        Direction direction, 
        IEnumerable<IDevice> monitoringDevices,
        WaveFormat waveFormat);
}