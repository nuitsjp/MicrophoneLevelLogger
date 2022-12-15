namespace MicrophoneLevelLogger.Domain;

public interface IMicrophone : IDisposable
{
    string Name { get; }
    float MasterVolumeLevelScalar { get; set; }
    float MasterPeakValue { get; }
    Task ActivateAsync();
    void StartRecording();
    IMasterPeakValues StopRecording();
    void Deactivate();
}