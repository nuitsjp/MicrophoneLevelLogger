namespace MicrophoneLevelLogger.Domain;

public class WaveInput
{
    public static readonly WaveInput Empty = 
        new(new[] { new DecibelByFrequency(0, 0) });
    public WaveInput(DecibelByFrequency[] decibelByFrequencies)
    {
        DecibelByFrequencies = decibelByFrequencies;
    }

    public DecibelByFrequency[] DecibelByFrequencies { get; }
    public double MaximumDecibel => DecibelByFrequencies.Max(x => x.Decibel);
}