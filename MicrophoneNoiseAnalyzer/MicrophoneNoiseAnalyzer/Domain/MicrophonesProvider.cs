namespace MicrophoneNoiseAnalyzer.Domain;

public class MicrophonesProvider : IMicrophonesProvider
{
    public IMicrophones Resolve() =>
        new Microphones();
}