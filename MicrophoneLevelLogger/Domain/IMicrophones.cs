namespace MicrophoneLevelLogger.Domain;

public interface IMicrophones : IDisposable
{
    IReadOnlyList<IMicrophone> Devices { get; }
    void Activate();
    void StartRecording();
    IEnumerable<IMasterPeakValues> StopRecording();
    void Deactivate();
    void DeleteRecordFiles();
}