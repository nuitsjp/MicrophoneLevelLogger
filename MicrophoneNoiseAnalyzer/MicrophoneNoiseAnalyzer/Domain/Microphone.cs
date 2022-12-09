using NAudio.CoreAudioApi;

namespace MicrophoneNoiseAnalyzer.Domain;

public class Microphone : IMicrophone
{
    private static readonly TimeSpan Period = TimeSpan.FromMilliseconds(50);

    private readonly MMDevice _mmDevice;

    private readonly List<float> _buffer = new();

    private readonly Timer _timer;

    public Microphone(MMDevice mmDevice)
    {
        _mmDevice = mmDevice;
        _timer = new Timer(OnElapsed, null, Timeout.InfiniteTimeSpan, Period);
    }

    public void Dispose()
    {
        _mmDevice.Dispose();
        _timer.Dispose();
    }

    public string Name => _mmDevice.FriendlyName;

    public float MasterVolumeLevelScalar
    {
        get => _mmDevice.AudioEndpointVolume.MasterVolumeLevelScalar;
        set => _mmDevice.AudioEndpointVolume.MasterVolumeLevelScalar = value;
    }

    public float MasterPeakValue => _mmDevice.AudioMeterInformation.MasterPeakValue;
    public IReadOnlyList<float> Buffer => _buffer;

    public void StartCapture()
    {
        _buffer.Clear();
        _timer.Change(TimeSpan.Zero, Period);
    }

    private void OnElapsed(object? state)
    {
        _buffer.Add(MasterPeakValue);
    }

    public void StopCapture()
    {
        _timer.Change(Timeout.InfiniteTimeSpan, Period);
    }
}