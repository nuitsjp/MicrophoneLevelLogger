using NAudio.Wave;

namespace Specter.Business;

public class Normalize
{
    private readonly double _maxValue;

    public Normalize(WaveFormat waveFormat)
    {
        _maxValue = Math.Pow(2, waveFormat.BitsPerSample - 1) - 1;
    }

    public double[] Filter(short[] samples)
    {
        var output = new double[samples.Length];
        for (int i = 0; i < samples.Length; i++)
        {
            output[i] = samples[i] / _maxValue;
        }

        return output;
    }
}