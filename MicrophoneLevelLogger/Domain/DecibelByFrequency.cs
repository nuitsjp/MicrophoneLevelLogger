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

    public override string ToString() => $"Frequency={Frequency}, Decibel={Decibel}";
}