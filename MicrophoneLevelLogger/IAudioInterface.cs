namespace MicrophoneLevelLogger;

public interface IAudioInterface : IDisposable
{
    VolumeLevel DefaultOutputLevel { get; set; }
    IEnumerable<IMicrophone> GetMicrophones(MicrophoneStatus status = MicrophoneStatus.Enable);
    IMediaPlayer GetMediaPlayer(bool isRemotePlay);
    void ActivateMicrophones();
}