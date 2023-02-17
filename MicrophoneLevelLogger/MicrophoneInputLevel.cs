namespace MicrophoneLevelLogger;

public class MicrophoneInputLevel
{
    public MicrophoneInputLevel(
        string id,
        string name,
        Decibel min,
        Decibel avg,
        Decibel max)
    {
        Id = id;
        Name = name;
        Min = min;
        Avg = avg;
        Max = max;
    }

    public string Id { get; }
    public string Name { get; }
    public Decibel Min { get; }
    public Decibel Avg { get; }
    public Decibel Max { get; }

}