namespace MicrophoneLevelLogger;

public interface IAudioInterface : IDisposable
{
    IEnumerable<IMicrophone> GetMicrophones(MicrophoneStatus status = MicrophoneStatus.Enable);
    Task<ISpeaker> GetSpeakerAsync();
    IEnumerable<ISpeaker> GetSpeakers();
    void ActivateMicrophones();
}