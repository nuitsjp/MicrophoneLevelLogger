using NAudio.CoreAudioApi;
using NAudio.Wave;
using System.Buffers;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using MMDeviceEnumerator = NAudio.CoreAudioApi.MMDeviceEnumerator;

namespace MicrophoneLevelLogger;

public class Microphone : IMicrophone
{
    private const double MaxSignal = 3.886626914120802;
    private const double Ratio = MaxSignal / short.MinValue * -1;

    private double[]? _lastBuffer;
    private readonly WaveInEvent _waveInEvent;

    private List<IObserver<WaveInput>> _observers = new();

    public Microphone(string id, string name, string systemName, int deviceNumber)
    {
        Id = id;
        Name = name;
        SystemName = systemName;
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

    private void WaveInEventOnDataAvailable(object? sender, WaveInEventArgs e)
    {
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


        try
        {
            var waveInput = new WaveInput(this, e.Buffer, e.BytesRecorded);
            _observers.ForEach(x => x.OnNext(waveInput));
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            throw;
        }
    }

    private void WaveInEventOnRecordingStopped(object? sender, StoppedEventArgs e)
    {
    }

    public void Dispose()
    {
        if (_lastBuffer is not null)
        {
            ArrayPool<double>.Shared.Return(_lastBuffer, true);
            _lastBuffer = null;
        }
    }

    public string Id { get; }
    public string Name { get; }
    public string SystemName { get; }
    public int DeviceNumber { get; }

    public WaveFormat WaveFormat => _waveInEvent.WaveFormat;

    public VolumeLevel VolumeLevel
    {
        get
        {
            using var mmDevice = GetMmDevice();
            return (VolumeLevel)mmDevice.AudioEndpointVolume.MasterVolumeLevelScalar;
        }
        set
        {
            var mmDevice = GetMmDevice();
            mmDevice.AudioEndpointVolume.MasterVolumeLevelScalar = (float)value;
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

    public void Deactivate()
    {
        _waveInEvent.StopRecording();
    }

    public IDisposable Subscribe(IObserver<WaveInput> observer)
    {
        ObserverDisposer observerDisposer = new (this, observer); 
        _observers.Add(observer);
        return observerDisposer;
    }

    private class ObserverDisposer : IDisposable
    {
        private readonly Microphone _microphone;
        private readonly IObserver<WaveInput> _observer;

        public ObserverDisposer(
            Microphone microphone, 
            IObserver<WaveInput> observer)
        {
            _microphone = microphone;
            _observer = observer;
        }

        public void Dispose()
        {

            var temp = _microphone._observers.ToList();
            temp.Remove(_observer);
            _microphone._observers = temp;
        }
    }

    public override string ToString() => Name;
}