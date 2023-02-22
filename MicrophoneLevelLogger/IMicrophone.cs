using NAudio.Wave;
using UnitGenerator;

namespace MicrophoneLevelLogger;

public interface IMicrophone
{
    public const double MinDecibel = -84;

    public const int SamplingMilliseconds = 125;

    MicrophoneId Id { get; }
    DeviceNumber DeviceNumber { get; }
    string Name { get; }
    string SystemName { get; }
    MicrophoneStatus Status { get; }
    VolumeLevel VolumeLevel { get; set; }
}

[UnitOf(typeof(int))]
public partial struct DeviceNumber
{
    
}