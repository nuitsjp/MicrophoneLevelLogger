using UnitGenerator;

namespace Specter.Business;

/// <summary>
/// 入出力レベル
/// </summary>
[UnitOf(
    typeof(float), 
    UnitGenerateOptions.Comparable | UnitGenerateOptions.Validate | UnitGenerateOptions.ArithmeticOperator | UnitGenerateOptions.JsonConverter)]
public readonly partial struct VolumeLevel
{
    /// <summary>
    /// 最小値
    /// </summary>
    private const float MinimumValue = 0f;
    /// <summary>
    /// 最大値
    /// </summary>
    private const float MaximumValue = 1.0f;

    /// <summary>
    /// 最小値
    /// </summary>
    public static readonly VolumeLevel Minimum = new(MinimumValue);
    /// <summary>
    /// 最大値
    /// </summary>
    public static readonly VolumeLevel Maximum = new(MaximumValue);

    /// <summary>
    /// 検証する
    /// </summary>
    /// <exception cref="Exception"></exception>
    private partial void Validate()
    {
        if (value < MinimumValue || MaximumValue < value)
        {
            throw new Exception("Invalid value range: " + value);
        }
    }
}