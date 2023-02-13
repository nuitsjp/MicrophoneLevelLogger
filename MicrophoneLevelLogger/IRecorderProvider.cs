namespace MicrophoneLevelLogger;

public interface IRecorderProvider
{
    IRecorder ResolveLocal();
    IRecorder ResolveRemote();
}