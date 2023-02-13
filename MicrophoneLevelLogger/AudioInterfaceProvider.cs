namespace MicrophoneLevelLogger;

public class AudioInterfaceProvider : IAudioInterfaceProvider
{
    public IAudioInterface Resolve() =>
        new AudioInterface();
}