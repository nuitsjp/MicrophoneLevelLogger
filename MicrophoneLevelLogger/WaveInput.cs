namespace MicrophoneLevelLogger;

public class WaveInput
{
    public WaveInput(IMicrophone source, byte[] buffer, int bytesRecorded, DecibelByFrequency[] decibelByFrequencies)
    {
        Source = source;
        Buffer = buffer;
        BytesRecorded = bytesRecorded;
        DecibelByFrequencies = decibelByFrequencies;
    }

    public IMicrophone Source { get; }
    public byte[] Buffer { get; }
    public int BytesRecorded { get; }
    public DecibelByFrequency[] DecibelByFrequencies { get; }
    public double MaximumDecibel => DecibelByFrequencies.Max(x => x.Decibel);
}