namespace MicrophoneLevelLogger.Domain;

public interface IAudioInterfaceProvider
{
    IAudioInterface Resolve();
}