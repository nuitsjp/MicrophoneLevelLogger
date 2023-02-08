namespace MicrophoneLevelLogger.Domain;

public class AudioInterfaceProvider : IAudioInterfaceProvider
{
    public IAudioInterface Resolve() =>
        new AudioInterface();
}