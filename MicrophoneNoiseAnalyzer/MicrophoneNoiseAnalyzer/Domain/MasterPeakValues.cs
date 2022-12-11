namespace MicrophoneNoiseAnalyzer.Domain;

public class MasterPeakValues : IMasterPeakValues
{
    public MasterPeakValues(IMicrophone microphone, IReadOnlyList<float> peakValues)
    {
        Microphone = microphone;
        PeakValues = peakValues;
    }

    public IMicrophone Microphone { get; }
    public IReadOnlyList<float> PeakValues { get; }
}