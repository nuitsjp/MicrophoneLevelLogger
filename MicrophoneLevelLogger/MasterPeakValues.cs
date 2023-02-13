namespace MicrophoneLevelLogger;

public class MasterPeakValues : IMasterPeakValues
{
    public MasterPeakValues(IMicrophone microphone, IReadOnlyList<double> peakValues)
    {
        Microphone = microphone;
        PeakValues = peakValues;
    }

    public IMicrophone Microphone { get; }
    public IReadOnlyList<double> PeakValues { get; }
}