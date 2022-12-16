using MicrophoneLevelLogger.Command;

namespace MicrophoneLevelLogger.Domain;

public interface IMicrophone : IDisposable
{
    string Name { get; }
    MasterVolumeLevelScalar MasterVolumeLevelScalar { get; set; }
    float MasterPeakValue { get; }
    Task ActivateAsync();
    void StartRecording();
    IMasterPeakValues StopRecording();
    void Deactivate();
}