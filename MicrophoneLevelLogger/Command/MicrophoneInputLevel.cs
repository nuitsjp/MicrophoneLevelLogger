namespace MicrophoneLevelLogger.Command;

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