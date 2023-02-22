namespace MicrophoneLevelLogger;

public interface ISpeaker
{
    SpeakerId Id { get; }
    string Name { get; }
    VolumeLevel VolumeLevel { get; set; }
}