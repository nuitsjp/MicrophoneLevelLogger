using System.Buffers;
using NAudio.Wave;

namespace MicrophoneLevelLogger;

public class Fft
{
    private const double MaxSignal = 3.886626914120802;
    private const double Ratio = MaxSignal / short.MinValue * -1;

    private readonly WaveFormat _waveFormat;
    private double[]? _lastBuffer;

    public Fft(WaveFormat waveFormat)
    {
        _waveFormat = waveFormat;
    }

    public DecibelByFrequency[] Transform(byte[] buffer, int bytesRecorded)
    {
        var bytesPerSample = _waveFormat.BitsPerSample / 8;
        var samplesRecorded = bytesRecorded / bytesPerSample;

        if (_lastBuffer is null)
        {
            _lastBuffer = ArrayPool<double>.Shared.Rent(samplesRecorded);
            // Rentされるサイズは2の階上になる。このとき0埋めされていない場合があるため、クリアしておく
            Array.Clear(_lastBuffer);
        }

        var indent = (_lastBuffer.Length - samplesRecorded) / 2;
        for (var i = 0; i < samplesRecorded; i++)
        {
            _lastBuffer[indent + i] = BitConverter.ToInt16(buffer, i * bytesPerSample) * Ratio;
        }

        var window = new FftSharp.Windows.Hanning();
        var windowed = window.Apply(_lastBuffer);
        var power = FftSharp.Transform.FFTpower(windowed);
        var frequencies = FftSharp.Transform.FFTfreq(_waveFormat.SampleRate, power.Length);
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

        return decibelByFrequencies;
    }
}