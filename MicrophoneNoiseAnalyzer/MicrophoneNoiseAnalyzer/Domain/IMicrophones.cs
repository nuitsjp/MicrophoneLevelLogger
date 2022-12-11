namespace MicrophoneNoiseAnalyzer.Domain;

public interface IMicrophones : IDisposable
{
    IReadOnlyList<IMicrophone> Devices { get; }
    void StartRecording();
    IEnumerable<IMasterPeakValues> StopRecording();
}