using UnitGenerator;

namespace MicrophoneLevelLogger;

[UnitOf(typeof(double), UnitGenerateOptions.Comparable | UnitGenerateOptions.JsonConverter | UnitGenerateOptions.ArithmeticOperator)]
public readonly partial struct Decibel
{
    public static readonly Decibel Min = new(-84);
    public static readonly Decibel Max = new(0);

    public static bool Validate(double decibel)
    {
        if(decibel < Min.AsPrimitive()) return false;
        return decibel <= Max.AsPrimitive();
    }
}