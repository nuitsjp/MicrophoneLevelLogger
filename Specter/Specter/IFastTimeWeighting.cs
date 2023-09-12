namespace Specter;

public interface IFastTimeWeighting
{
    double[] Filter(double[] input);
}