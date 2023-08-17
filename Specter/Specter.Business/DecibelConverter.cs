namespace Quietrum;

public class DecibelConverter
{
    public double Convert(double[] samples)
    {
        var totalPower = 0.0;

        foreach (var volumeInDb in samples)
        {
            // Convert decibel to power
            var power = Math.Pow(10, volumeInDb / 10);

            // Add power to total
            totalPower += power;
        }

        // Convert total power back to decibels
        var decibel = 10 * Math.Log10(totalPower);
        if (double.IsNaN(decibel) 
            || double.IsNegativeInfinity(decibel)
            || decibel < Decibel.MinimumValue)
        {
            decibel = Decibel.MinimumValue;
        }
        
        return decibel;
    }

}