namespace Quietrum;

public class WaveInput
{
    public WaveInput(byte[] buffer, int bytesRecorded)
    {
        Buffer = buffer;
        BytesRecorded = bytesRecorded;
    }

    public byte[] Buffer { get; }
    public int BytesRecorded { get; }
}
