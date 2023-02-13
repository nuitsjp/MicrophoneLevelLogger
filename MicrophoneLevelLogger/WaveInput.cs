namespace MicrophoneLevelLogger;

public class WaveInput
{
    public static readonly WaveInput Empty =
        new(new[] { new DecibelByFrequency(0, IMicrophone.MinDecibel) });
    public WaveInput(DecibelByFrequency[] decibelByFrequencies)
    {
        DecibelByFrequencies = decibelByFrequencies;
    }

    public DecibelByFrequency[] DecibelByFrequencies { get; }
    public double MaximumDecibel => DecibelByFrequencies.Max(x => x.Decibel);
}