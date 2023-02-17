namespace MicrophoneLevelLogger;

public class WaveInput
{
    public WaveInput(IMicrophone source, byte[] buffer, int bytesRecorded)
    {
        Source = source;
        Buffer = buffer;
        BytesRecorded = bytesRecorded;
    }

    public IMicrophone Source { get; }
    public byte[] Buffer { get; }
    public int BytesRecorded { get; }
}