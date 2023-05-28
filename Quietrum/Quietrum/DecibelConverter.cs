namespace Quietrum;

public class DecibelConverter
{
    public double Convert(double[] samples)
    {
        double rms = 0;

        foreach (var sample in samples)
        {
            rms += sample * sample;
        }
    
        rms = Math.Sqrt(rms / samples.Length);

        double decibel = 20 * Math.Log10(rms);
    
        // Prevent NaN due to log10(0)
        if (double.IsNaN(decibel) 
            || double.IsNegativeInfinity(decibel)
            || decibel < Decibel.MinimumValue)
        {
            decibel = Decibel.MinimumValue;
        }

        return decibel;
    }

}