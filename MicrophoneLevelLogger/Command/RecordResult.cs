using MicrophoneLevelLogger.Domain;

namespace MicrophoneLevelLogger.Command;

public class RecordResult
{
    public RecordResult(int no, IMasterPeakValues masterPeakValues)
    {
        No = no;
        Name = masterPeakValues.Microphone.Name;
        Min = masterPeakValues.PeakValues.Any() ? masterPeakValues.PeakValues.Min() : IMicrophone.MinDecibel;
        Avg = masterPeakValues.PeakValues.Any() ? masterPeakValues.PeakValues.Average() : IMicrophone.MinDecibel;
        Median = masterPeakValues.PeakValues.Any() ? masterPeakValues.PeakValues.Median() : IMicrophone.MinDecibel;
        Max = masterPeakValues.PeakValues.Any() ? masterPeakValues.PeakValues.Max() : IMicrophone.MinDecibel;
    }
    public int No { get; }
    public string Name { get; }
    public double Min { get; }
    public double Avg { get; }
    public double Median { get; }
    public double Max { get; }
}