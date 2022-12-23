namespace MicrophoneLevelLogger.Domain;

public readonly struct DecibelByFrequency
{
    public readonly double Frequency;
    public readonly double Decibel;

    public DecibelByFrequency(double frequency, double decibel)
    {
        Frequency = frequency;
        Decibel = decibel;
    }
}