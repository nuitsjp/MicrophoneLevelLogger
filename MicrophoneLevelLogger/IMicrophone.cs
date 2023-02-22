using NAudio.Wave;

namespace MicrophoneLevelLogger;

public interface IMicrophone : IObservable<WaveInput>, IDisposable
{
    public const double MinDecibel = -84;

    public const int SamplingMilliseconds = 125;

    MicrophoneId Id { get; }
    string Name { get; }
    string SystemName { get; }
    MicrophoneStatus Status { get; }
    VolumeLevel VolumeLevel { get; set; }
    WaveFormat WaveFormat { get; }
    Task ActivateAsync();
    void Deactivate();
}

[Flags]
public enum MicrophoneStatus
{
    Disable = 0x01,
    Enable = 0x02
}