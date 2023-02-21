namespace MicrophoneLevelLogger;

public interface IAudioInterface : IDisposable
{
    VolumeLevel DefaultOutputLevel { get; set; }
    IReadOnlyList<IMicrophone> Microphones { get; }
    IReadOnlyList<IMicrophone> AllMicrophones { get; }
    void ActivateMicrophones();
    void DeactivateMicrophones();

}