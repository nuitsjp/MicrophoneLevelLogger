namespace MicrophoneLevelLogger.Domain;

public interface IMicrophone : IDisposable
{
    public const double MinDecibel = -84;

    event EventHandler<WaveInput> DataAvailable;

    string Id { get; }
    string Name { get; }
    int DeviceNumber { get; }
    WaveInput LatestWaveInput { get; }
    MasterVolumeLevelScalar MasterVolumeLevelScalar { get; set; }
    Task ActivateAsync();
    void StartRecording();
    IMasterPeakValues StopRecording();
    void Deactivate();
}