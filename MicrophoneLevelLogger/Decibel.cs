using UnitGenerator;

namespace MicrophoneLevelLogger;

[UnitOf(typeof(double), UnitGenerateOptions.Comparable)]
public readonly partial struct Decibel
{
    public static readonly Decibel Min = new(-84);
    public static readonly Decibel Max = new(0);
}