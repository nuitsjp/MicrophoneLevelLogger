using UnitGenerator;

namespace MicrophoneLevelLogger;

/// <summary>
/// デシベル。音量の単位。
/// </summary>
[UnitOf(typeof(double), UnitGenerateOptions.Comparable | UnitGenerateOptions.JsonConverter | UnitGenerateOptions.ArithmeticOperator)]
public readonly partial struct Decibel
{
    /// <summary>
    /// 最小値
    /// </summary>
    public static readonly Decibel Minimum = new(-84);
    /// <summary>
    /// 最大値
    /// </summary>
    public static readonly Decibel Maximum = new(0);

    /// <summary>
    /// 値を検証する。
    /// </summary>
    /// <param name="decibel"></param>
    /// <returns></returns>
    public static bool Validate(double decibel)
    {
        if(decibel < Minimum.AsPrimitive()) return false;
        return decibel <= Maximum.AsPrimitive();
    }
}