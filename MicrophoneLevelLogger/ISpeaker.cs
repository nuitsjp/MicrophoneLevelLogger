namespace MicrophoneLevelLogger;

public interface ISpeaker
{
    SpeakerId Id { get; }
    string Name { get; }
}