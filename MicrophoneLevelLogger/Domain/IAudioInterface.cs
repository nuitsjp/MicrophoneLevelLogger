namespace MicrophoneLevelLogger.Domain;

public interface IAudioInterface : IDisposable
{
    IReadOnlyList<IMicrophone> Microphones { get; }
    void ActivateMicrophones();
    void StartRecording(string path);
    IEnumerable<IMasterPeakValues> StopRecording();
    void DeactivateMicrophones();

}