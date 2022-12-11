namespace MicrophoneNoiseAnalyzer.Domain;

public interface IMicrophone : IDisposable
{
    string Name { get; }
    float MasterVolumeLevelScalar { get; set; }
    float MasterPeakValue { get; }
    void StartRecording();
    IMasterPeakValues StopRecording();
}