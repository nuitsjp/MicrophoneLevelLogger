namespace MicrophoneLevelLogger.Domain;

public class MicrophonesProvider : IMicrophonesProvider
{
    public IMicrophones Resolve() =>
        new Microphones();
}