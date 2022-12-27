namespace MicrophoneLevelLogger.Domain;

public interface IMicrophone : IDisposable
{
    public const double MinDecibel = -84;

    public const int SamplingMilliseconds = 125;

    event EventHandler<WaveInput> DataAvailable;

    string Id { get; }
    string Name { get; }
    int DeviceNumber { get; }
    WaveInput LatestWaveInput { get; }
    MasterVolumeLevelScalar MasterVolumeLevelScalar { get; set; }
    Task ActivateAsync();
    void StartRecording(string path);
    IMasterPeakValues StopRecording();
    void Deactivate();
}