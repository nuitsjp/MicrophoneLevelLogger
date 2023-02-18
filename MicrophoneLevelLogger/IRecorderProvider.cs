namespace MicrophoneLevelLogger;

public interface IRecorderProvider
{
    IRecorder ResolveLocal(IAudioInterface audioInterface, string? recordName);
    IRecorder ResolveLocal(params IMicrophone[] microphones);
    IRecorder ResolveRemote(string? recordName);
}