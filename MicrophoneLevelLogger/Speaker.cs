namespace MicrophoneLevelLogger;

public class Speaker : ISpeaker
{
    public Speaker(SpeakerId id, string name)
    {
        Id = id;
        Name = name;
    }

    public SpeakerId Id { get; }
    public string Name { get; }
}