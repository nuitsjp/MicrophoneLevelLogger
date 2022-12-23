using MicrophoneLevelLogger.Command;

namespace MicrophoneLevelLogger.Domain;

public interface IMicrophone : IDisposable
{
    public const double MinDecibel = -84;

    event EventHandler<WaveInput> DataAvailable;

    string Name { get; }
    WaveInput LatestWaveInput { get; }
    MasterVolumeLevelScalar MasterVolumeLevelScalar { get; set; }
    Task ActivateAsync();
    void StartRecording();
    IMasterPeakValues StopRecording();
    void Deactivate();
}