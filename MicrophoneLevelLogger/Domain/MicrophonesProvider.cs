namespace MicrophoneLevelLogger.Domain;

public class MicrophonesProvider : IMicrophonesProvider
{
    public IAudioInterface Resolve() =>
        new AudioInterface();
}