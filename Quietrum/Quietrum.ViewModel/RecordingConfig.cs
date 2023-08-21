using NAudio.Wave;
using Specter.Business;

namespace Quietrum.ViewModel;

public record RecordingConfig(
    WaveFormat WaveFormat,
    TimeSpan RecordingSpan,
    RefreshRate RefreshRate)
{
    public static readonly RecordingConfig Default =
        new(new (48_000, 16, 1), TimeSpan.FromMinutes(2), new RefreshRate(40));

    public int RecordingLength => (int)(RecordingSpan / RefreshRate.Interval);
}