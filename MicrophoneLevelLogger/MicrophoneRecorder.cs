using NAudio.Wave;
using System.Buffers;
using System.IO;

namespace MicrophoneLevelLogger;

public class MicrophoneRecorder : IMicrophoneRecorder
{
    private const double MaxSignal = 3.886626914120802;
    private const double Ratio = MaxSignal / short.MinValue * -1;

    private readonly DirectoryInfo? _directoryInfo;

    public MicrophoneRecorder(IMicrophone microphone, DirectoryInfo? directoryInfo)
    {
        Microphone = microphone;
        _directoryInfo = directoryInfo;

    }
    public IMicrophone Microphone { get; }

    public Decibel Max { get; private set; } = Decibel.Min;
    public Decibel Avg { get; private set; } = Decibel.Min;
    public Decibel Min { get; private set; } = Decibel.Min;

    public Task StartAsync(CancellationToken token)
    {
        var waveInEvent = new WaveInEvent
        {
            DeviceNumber = Microphone.DeviceNumber.AsPrimitive(),
            WaveFormat = new WaveFormat(rate: 48_000, bits: 16, channels: 1),
            BufferMilliseconds = IMicrophone.SamplingMilliseconds
        };
        Fft fft = new(waveInEvent.WaveFormat);
        var waveWriter = _directoryInfo is not null
            ? new WaveFileWriter(Path.Join(_directoryInfo.FullName, $"{Microphone.Name}.wav"), waveInEvent.WaveFormat)
            : Stream.Null;

        var bytesPerSample = waveInEvent.WaveFormat.BitsPerSample / 8;
        double[]? buffer = null;

        waveInEvent.DataAvailable += (sender, e) =>
        {
            var samplesRecorded = e.BytesRecorded / bytesPerSample;

            if (buffer is null)
            {
                buffer = ArrayPool<double>.Shared.Rent(samplesRecorded);
                // Rentされるサイズは2の階上になる。このとき0埋めされていない場合があるため、クリアしておく
                Array.Clear(buffer);
            }

            var indent = (buffer.Length - samplesRecorded) / 2;
            for (var i = 0; i < samplesRecorded; i++)
            {
                buffer[indent + i] = BitConverter.ToInt16(e.Buffer, i * bytesPerSample) * Ratio;
            }


            waveWriter.Write(e.Buffer, 0, e.BytesRecorded);
            var decibels =
                AWeighting.Instance.Filter(
                    fft.Transform(e.Buffer, e.BytesRecorded));
            Min = new Decibel(decibels.Min(x => x.Decibel));
            Avg = new Decibel(decibels.Average(x => x.Decibel));
            Max = new Decibel(decibels.Max(x => x.Decibel));
        };

        waveInEvent.StartRecording();

        void Callback()
        {
            waveInEvent.StopRecording();
            waveInEvent.Dispose();

            // ReSharper disable once MethodSupportsCancellation
            waveWriter.Flush();
            waveWriter.Dispose();
        }

        token.Register(Callback);

        return Task.CompletedTask;
    }
}