namespace MicrophoneNoiseAnalyzer.Domain;

public interface IMicrophonesProvider
{
    IMicrophones Resolve();
}