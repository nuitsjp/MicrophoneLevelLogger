using NAudio.Wave;

namespace Quietrum.ViewModel;

public record RecordingConfig(
    WaveFormat WaveFormat,
    TimeSpan RecordingSpan,
    TimeSpan RecordingInterval)
{
    public static readonly RecordingConfig Default =
        new(new (48_000, 16, 1), TimeSpan.FromMinutes(2), TimeSpan.FromMilliseconds(50));

    public int RecordingLength => (int)(RecordingSpan / RecordingInterval);
    public int BytesPerSample => WaveFormat.BitsPerSample / 8;
}