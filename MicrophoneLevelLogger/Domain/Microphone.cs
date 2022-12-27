using NAudio.CoreAudioApi;
using NAudio.Wave;
using System.Buffers;
using System.Xml.Linq;
using MMDeviceEnumerator = NAudio.CoreAudioApi.MMDeviceEnumerator;

namespace MicrophoneLevelLogger.Domain;

public class Microphone : IMicrophone
{
    private static readonly TimeSpan SamplingRate = TimeSpan.FromMilliseconds(50);

    private readonly WaveInEvent _waveInEvent;
    private double[]? _lastBuffer;
    private WaveFileWriter? _waveFileWriter;

    private readonly List<double> _masterPeakBuffer = new();

    public Microphone(string id, string name, int deviceNumber)
    {
        Id = id;
        Name = name;
        DeviceNumber = deviceNumber;

        _waveInEvent = new WaveInEvent
        {
            DeviceNumber = deviceNumber,
            WaveFormat = new WaveFormat(rate: 48_000, bits: 16, channels: 1),
            BufferMilliseconds = IMicrophone.SamplingMilliseconds
        };
        _waveInEvent.DataAvailable += WaveInEventOnDataAvailable;
        _waveInEvent.RecordingStopped += WaveInEventOnRecordingStopped;
    }

    private const double MaxSignal = 3.886626914120802;
    private const double Ratio = MaxSignal / short.MinValue * -1;

    private void WaveInEventOnDataAvailable(object? sender, WaveInEventArgs e)
    {
        // レコーディング中であればファイルに保存する
        _waveFileWriter?.Write(e.Buffer, 0, e.BytesRecorded);

        var bytesPerSample = _waveInEvent.WaveFormat.BitsPerSample / 8;
        var samplesRecorded = e.BytesRecorded / bytesPerSample;

        if (_lastBuffer is null)
        {
            _lastBuffer = ArrayPool<double>.Shared.Rent(samplesRecorded);
            // Rentされるサイズは2の階上になる。このとき0埋めされていない場合があるため、クリアしておく
            Array.Clear(_lastBuffer);
        }

        var indent = (_lastBuffer.Length - samplesRecorded) / 2;
        for (var i = 0; i < samplesRecorded; i++)
        {
            _lastBuffer[indent + i] = BitConverter.ToInt16(e.Buffer, i * bytesPerSample) * Ratio;
        }

        var window = new FftSharp.Windows.Hanning();
        var windowed = window.Apply(_lastBuffer);
        var power = FftSharp.Transform.FFTpower(windowed);
        var frequencies = FftSharp.Transform.FFTfreq(_waveInEvent.WaveFormat.SampleRate, power.Length);
        var decibelByFrequencies = new DecibelByFrequency[power.Length];
        for (var i = 0; i < power.Length; i++)
        {
            decibelByFrequencies[i] = new DecibelByFrequency(
                frequencies[i],
                power[i] < IMicrophone.MinDecibel
                    ? IMicrophone.MinDecibel
                    : power[i]
            );
        }

        try
        {
            var weighted = AWeighting.Instance.Filter(decibelByFrequencies);

            LatestWaveInput = new(weighted);
            DataAvailable?.Invoke(this, LatestWaveInput);

            _masterPeakBuffer.Add(LatestWaveInput.MaximumDecibel);
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            throw;
        }
    }

    private void WaveInEventOnRecordingStopped(object? sender, StoppedEventArgs e)
    {
        FinalizeWaveFile();
    }

    private void FinalizeWaveFile()
    {
        _waveFileWriter?.Flush();
        _waveFileWriter?.Dispose();
        _waveFileWriter = null;
    }

    public void Dispose()
    {
        _waveInEvent.DisposeQuiet();
    }


    public event EventHandler<WaveInput>? DataAvailable;

    public string Id { get; }
    public string Name { get; }
    public int DeviceNumber { get; }
    public WaveInput LatestWaveInput { get; private set; } = WaveInput.Empty;

    public MasterVolumeLevelScalar MasterVolumeLevelScalar
    {
        get
        {
            using var mmDevice = GetMmDevice();
            return (MasterVolumeLevelScalar) mmDevice.AudioEndpointVolume.MasterVolumeLevelScalar;
        }
        set
        {
            var mmDevice = GetMmDevice();
            mmDevice.AudioEndpointVolume.MasterVolumeLevelScalar = (float) value;
        }
    }

    private MMDevice GetMmDevice()
    {
        using var enumerator = new MMDeviceEnumerator();
        return enumerator.GetDevice(Id);
    }

    public Task ActivateAsync()
    {
        _waveInEvent.StartRecording();
        return Task.CompletedTask;
    }

    public void StartRecording(string path)
    {
        _masterPeakBuffer.Clear();
        _waveFileWriter = new(Path.Combine(path, Name + ".wav"), _waveInEvent.WaveFormat);
    }


    private void OnElapsed(object? state)
    {
        _masterPeakBuffer.Add(LatestWaveInput.MaximumDecibel);
    }

    public IMasterPeakValues StopRecording()
    {
        FinalizeWaveFile();

        return new MasterPeakValues(this, _masterPeakBuffer.ToList());
    }

    public void Deactivate()
    {
        StopRecording();
        _waveInEvent.StopRecording();
    }

    public override string ToString() => Name;
}