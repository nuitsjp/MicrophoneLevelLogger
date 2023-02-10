namespace MicrophoneLevelLogger.Domain;

public interface IRecorderProvider
{
    IRecorder ResolveLocal();
    IRecorder ResolveRemote();
}