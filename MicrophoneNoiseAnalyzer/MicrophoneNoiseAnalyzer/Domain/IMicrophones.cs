namespace MicrophoneNoiseAnalyzer.Domain;

public interface IMicrophones : IDisposable
{
    IReadOnlyList<IMicrophone> Devices { get; }
    void StartCapture();
    void StopCapture();
}