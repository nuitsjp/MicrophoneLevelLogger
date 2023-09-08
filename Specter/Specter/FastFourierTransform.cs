using FftSharp.Windows;
using NAudio.Wave;

namespace Specter;

/// <summary>
/// From:ChatGPT
/// リアルタイムの音声解析を行う場合、適切なFFT長は一部の要素に依存します：
/// * 解析の精度：より高い周波数分解能が必要な場合、FFT長を大きくすることが望ましいです。ただし、これは計算の複雑さとリソース使用量を増加させます。
/// * 計算能力：より高いFFT長はより高い計算能力を必要とします。利用可能なリソースとパフォーマンス要件に基づいて適切なFFT長を選択する必要があります。
/// * リアルタイム要件：より低い時間解像度（つまり、より高速な更新）が必要な場合、FFT長を小さくすることが望ましいです。
/// 
/// 一般的には、リアルタイムの音声解析の場合、512または1024のFFT長がよく使用されます。
/// これらの値は、計算量と解析精度の間の妥当なトレードオフを提供します。
/// より高いFFT長（例えば2048や4096）は、より高い周波数分解能を提供しますが、計算量が増加し、リアルタイムのパフォーマンスが低下する可能性があります。
/// 
/// また、あなたの目的である「人間の耳による音の聞こえ方を分析する」を考慮すると、FFT長は人間の聴覚の特性により影響を受ける可能性があります。
/// 人間の耳は特定の周波数範囲（約20Hzから20kHz）に感度が高く、この範囲内での解析精度が重要となります。
/// したがって、この範囲をカバーするために適切なFFT長を選択することが重要です。
/// たとえば、48kHzのサンプリングレートを使用している場合、1024のFFT長は約21Hzの周波数分解能を提供します。
/// これは人間の聴覚範囲の下限（約20Hz）に近いです。
/// したがって、あなたのケースでは、FFT長として512または1024を推奨します。
/// ただし、これらの値は初期の推定値であり、実際のパフォーマンスと要件に基づいて調整することができます。
/// </summary>
public class FastFourierTransform
{
    private static readonly Hanning Hanning = new();

    private readonly double _sampleRate;

    public FastFourierTransform(WaveFormat waveFormat)
    {
        _sampleRate = waveFormat.SampleRate;
    }

    public double[] Transform(double[] signal)
    {
        // Shape the signal using a Hanning window
        Hanning.ApplyInPlace(signal);

        // Calculate the FFT as an array of complex numbers
        System.Numerics.Complex[] spectrum = FftSharp.FFT.Forward(signal);
        var power  = FftSharp.FFT.Power(spectrum);
        double[] freq = FftSharp.FFT.FrequencyScale(power.Length, _sampleRate);

        return AWeighting.Filter(power, freq);
    }
}