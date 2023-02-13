using MicrophoneLevelLogger.Domain;

namespace MicrophoneLevelLogger;

public interface IMicrophone : IDisposable
{
    public const double MinDecibel = -84;

    public const int SamplingMilliseconds = 125;

    event EventHandler<WaveInput> DataAvailable;

    string Id { get; }
    string Name { get; }
    int DeviceNumber { get; }
    WaveInput LatestWaveInput { get; }
    VolumeLevel VolumeLevel { get; set; }
    Task ActivateAsync();
    void StartRecording(string path);
    IMasterPeakValues StopRecording();
    void Deactivate();
}