namespace MicrophoneLevelLogger.Domain;

public interface IMasterPeakValues
{
    IMicrophone Microphone { get; }
    IReadOnlyList<float> PeakValues { get; }
}