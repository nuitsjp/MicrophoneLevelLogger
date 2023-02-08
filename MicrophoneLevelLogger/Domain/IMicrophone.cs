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

public class MicrophoneInputLevels
{
    private IList<MicrophoneInputLevel> Microphones { get; set; } = new List<MicrophoneInputLevel>();
}

public class MicrophoneInputLevel
{
    public MicrophoneInputLevel(
        string id, 
        string name, 
        double min, 
        double avg, 
        double median, 
        double max)
    {
        Id = id;
        Name = name;
        Min = min;
        Avg = avg;
        Median = median;
        Max = max;
    }

    public string Id { get; }
    public string Name { get; }
    public double Min { get; }
    public double Avg { get; }
    public double Median { get; }
    public double Max { get; }

}