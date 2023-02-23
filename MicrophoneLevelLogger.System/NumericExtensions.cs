namespace MicrophoneLevelLogger;

public static class NumericExtensions
{
    public static bool Between(this float value, float begin, float end)
    {
        if (value < begin) return false;
        return !(end < value);
    }
}