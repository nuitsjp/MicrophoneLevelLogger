namespace MicrophoneNoiseAnalyzer.Domain;

public interface IMicrophone : IDisposable
{
    string Name { get; }
    float MasterVolumeLevelScalar { get; set; }
    float MasterPeakValue { get; }
    void Activate();
    void StartRecording();
    IMasterPeakValues StopRecording();
    void Deactivate();
}