namespace MicrophoneLevelLogger.Domain;

public interface IMicrophones : IDisposable
{
    IReadOnlyList<IMicrophone> Devices { get; }
    void Activate();
    void StartRecording();
    IEnumerable<IMasterPeakValues> StopRecording();
    public void Calibrate();
    void Deactivate();

}