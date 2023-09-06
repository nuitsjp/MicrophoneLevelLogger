using NAudio.Wave;
using Specter.Business;

namespace Specter.Repository;

public class AudioRecorderProvider : IAudioRecorderProvider
{
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
            waveFormat);
    }
}