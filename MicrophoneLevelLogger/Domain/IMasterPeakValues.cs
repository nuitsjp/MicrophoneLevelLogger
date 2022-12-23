namespace MicrophoneLevelLogger.Domain;

public interface IMasterPeakValues
{
    IMicrophone Microphone { get; }
    IReadOnlyList<double> PeakValues { get; }
}