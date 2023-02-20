namespace MicrophoneLevelLogger;

public class MicrophoneRecordSummary
{
    public MicrophoneRecordSummary(
        MicrophoneId id, 
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

    public MicrophoneId Id { get; }
    public string Name { get; }
    public Decibel Min { get; }
    public Decibel Avg { get; }
    public Decibel Max { get; }

}