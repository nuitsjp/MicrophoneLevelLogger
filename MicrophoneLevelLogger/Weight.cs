namespace MicrophoneLevelLogger;

public readonly struct Weight
{
    public readonly double Frequency;
    public readonly double Value;

    public Weight(double frequency, double value)
    {
        Frequency = frequency;
        Value = value;
    }
}