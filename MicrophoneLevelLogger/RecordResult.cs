namespace MicrophoneLevelLogger;

public class RecordResult
{
    public RecordResult(int no, IMicrophoneRecorder microphoneRecorder)
    {
        No = no;
        Name = microphoneRecorder.Microphone.Name;
        Min = microphoneRecorder.Min;
        Avg = microphoneRecorder.Avg;
        Max = microphoneRecorder.Max;
    }

    public RecordResult(int no, string name, Decibel min, Decibel avg, Decibel max)
    {
        No = no;
        Name = name;
        Min = min;
        Avg = avg;
        Max = max;
    }

    public int No { get; }
    public string Name { get; }
    public Decibel Min { get; }
    public Decibel Avg { get; }
    public Decibel Max { get; }
}