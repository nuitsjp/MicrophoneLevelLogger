using System.Buffers;
using System.Diagnostics;
using NAudio.Wave;

namespace MicrophoneLevelLogger;

/// <summary>
/// 高速フーリエ変換
/// </summary>
public class Fft
{
    /// <summary>
    /// このツールで利用するWaveフォーマットにおける最大値
    /// </summary>
    private const double MaxSignal = 3.886626914120802;

    /// <summary>
    /// 音量計算の補正値（本当は間違っているかもしれない。自信がない）
    /// </summary>
    private const double Ratio = MaxSignal / short.MinValue * -1;

    /// <summary>
    /// WaveFormat
    /// </summary>
    private readonly WaveFormat _waveFormat;

    /// <summary>
    /// インスタンスを生成する。
    /// </summary>
    /// <param name="waveFormat"></param>
    public Fft(WaveFormat waveFormat)
    {
        _waveFormat = waveFormat;
    }

    /// <summary>
    /// 高速フーリエ変換を実行する
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="bytesRecorded"></param>
    /// <returns></returns>

    public DecibelByFrequency[] Transform(byte[] buffer, int bytesRecorded)
    {
        var bytesPerSample = _waveFormat.BitsPerSample / 8;
        var samplesRecorded = bytesRecorded / bytesPerSample;

        var adjusted = ArrayPool<double>.Shared.Rent(samplesRecorded);
        try
        {
            Array.Clear(adjusted);

            var indent = (adjusted.Length - samplesRecorded) / 2;
            for (var i = 0; i < samplesRecorded; i++)
            {
                adjusted[indent + i] = BitConverter.ToInt16(buffer, i * bytesPerSample) * Ratio;
            }

            var window = new FftSharp.Windows.Hanning();
            var windowed = window.Apply(adjusted);
            var power = FftSharp.Transform.FFTpower(windowed);
            var frequencies = FftSharp.Transform.FFTfreq(_waveFormat.SampleRate, power.Length);
            var decibelByFrequencies = new DecibelByFrequency[power.Length];
            for (var i = 0; i < power.Length; i++)
            {
                var decibel = (Decibel)power[i];
                decibelByFrequencies[i] = new DecibelByFrequency(
                    (Hz)frequencies[i],
                    decibel < Decibel.Minimum
                        ? Decibel.Minimum
                        : decibel
                );
            }

            return decibelByFrequencies;
        }
        finally
        {
            ArrayPool<double>.Shared.Return(adjusted);
        }
    }
}