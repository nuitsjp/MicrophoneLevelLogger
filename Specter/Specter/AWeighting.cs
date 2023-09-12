namespace Specter;

/// <summary>
/// 周波数重み付け特性「A特性」
/// </summary>
public class AWeighting : IAWeighting
{
    private static readonly (double Frequency, double Correction)[] AWeightingTable = new[]
    {
        (6.3, -85.4),
        (8, -77.6),
        (10, -70.4),
        (12.5, -63.6),
        (16, -56.4),
        (20, -50.4),
        (25, -44.8),
        (31.5, -39.5),
        (40, -34.5),
        (50, -30.3),
        (63, -26.2),
        (80, -22.4),
        (100, -19.1),
        (125, -16.2),
        (160, -13.2),
        (200, -10.8),
        (250, -8.7),
        (315, -6.6),
        (400, -4.8),
        (500, -3.2),
        (630, -1.9),
        (800, -0.8),
        (1000, 0.0),
        (1250, 0.6),
        (1600, 1.0),
        (2000, 1.2),
        (2500, 1.3),
        (3150, 1.2),
        (4000, 1.0),
        (5000, 0.6),
        (6300, -0.1),
        (8000, -1.1),
        (10000, -2.5),
        (12500, -4.3),
        (16000, -6.7),
        (20000, -9.3)
    };
    
    public double[] Filter(double[] dbArray, double[] freqArray)
    {
        double[] result = new double[dbArray.Length];
        for (int i = 0; i < dbArray.Length; i++)
        {
            double closestCorrection = FindClosestCorrection(freqArray[i]);
            result[i] = dbArray[i] + closestCorrection;
        }
        return result;
    }

    private static double FindClosestCorrection(double freq)
    {
        for (int i = 0; i < AWeightingTable.Length - 1; i++)
        {
            if (freq >= AWeightingTable[i].Frequency && freq < AWeightingTable[i + 1].Frequency)
            {
                return AWeightingTable[i].Correction;
            }
        }

        // For frequencies larger than the last element in the table, return the correction of the last element
        return AWeightingTable[^1].Correction;
    }
}
