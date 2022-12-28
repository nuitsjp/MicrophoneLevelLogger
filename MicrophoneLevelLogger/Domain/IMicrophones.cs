namespace MicrophoneLevelLogger.Domain;

public interface IMicrophones : IDisposable
{
    IReadOnlyList<IMicrophone> Devices { get; }
    void Activate();
    void StartRecording(string path);
    IEnumerable<IMasterPeakValues> StopRecording();
    void Deactivate();
}