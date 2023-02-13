namespace MicrophoneLevelLogger;

public interface IMasterPeakValues
{
    IMicrophone Microphone { get; }
    IReadOnlyList<double> PeakValues { get; }
}