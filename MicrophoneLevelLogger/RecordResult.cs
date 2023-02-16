namespace MicrophoneLevelLogger;

public class RecordResult
{
    public RecordResult(int no, IMicrophoneLogger microphoneLogger)
    {
        No = no;
        Name = microphoneLogger.Microphone.Name;
        Min = microphoneLogger.Min;
        Avg = microphoneLogger.Avg;
        Max = microphoneLogger.Max;
    }
    public int No { get; }
    public string Name { get; }
    public Decibel Min { get; }
    public Decibel Avg { get; }
    public Decibel Max { get; }
}