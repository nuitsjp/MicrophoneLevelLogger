using NAudio.Wave;

namespace MicrophoneLevelLogger;

public interface IMicrophone : IObservable<WaveInput>, IDisposable
{
    public const double MinDecibel = -84;

    public const int SamplingMilliseconds = 125;

    string Id { get; }
    string Name { get; }
    int DeviceNumber { get; }
    WaveInput LatestWaveInput { get; }
    VolumeLevel VolumeLevel { get; set; }
    WaveFormat WaveFormat { get; }
    Task ActivateAsync();
    void Deactivate();
}