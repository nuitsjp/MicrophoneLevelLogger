namespace Specter;

public interface IAWeighting
{
    double[] Filter(double[] dbArray, double[] freqArray);
}