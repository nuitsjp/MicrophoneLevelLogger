namespace MicrophoneLevelLogger;

public interface IAudioInterfaceProvider
{
    IAudioInterface Resolve();
}