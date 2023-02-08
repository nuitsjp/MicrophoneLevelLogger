namespace MicrophoneLevelLogger.Domain;

public interface IMicrophonesProvider
{
    IAudioInterface Resolve();
}