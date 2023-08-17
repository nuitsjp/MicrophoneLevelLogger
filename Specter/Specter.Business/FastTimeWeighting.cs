using NAudio.Wave;

namespace Quietrum;

public class FastTimeWeighting
{
    /// <summary>
    /// time constant for Fast Time Weighting
    /// </summary>
    private const double Tau = 0.125;

    private readonly double _alpha;

    private double _lastOutput;

    public FastTimeWeighting(WaveFormat samplingRate)
    {
        _alpha = Tau / (Tau + (1.0d / samplingRate.SampleRate));
    }

    public double[] Filter(double[] input)
    {
        for (int i = 0; i < input.Length; i++)
        {
            var output = _alpha * input[i] + (1 - _alpha) * _lastOutput;
            _lastOutput = output;
            input[i] = _lastOutput;
        }
        return input;
    }
}