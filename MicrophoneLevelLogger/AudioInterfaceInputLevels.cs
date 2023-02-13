namespace MicrophoneLevelLogger;

public class AudioInterfaceInputLevels
{
    public IList<MicrophoneInputLevel> Microphones { get; set; } = new List<MicrophoneInputLevel>();

    public void Update(MicrophoneInputLevel microphoneInputLevel)
    {
        var old = Microphones.SingleOrDefault(x => x.Name == microphoneInputLevel.Name);
        if (old is not null)
        {
            Microphones.Remove(old);
        }

        Microphones.Add(microphoneInputLevel);
    }
}