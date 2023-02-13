using UnitGenerator;

namespace MicrophoneLevelLogger;

[UnitOf(
    typeof(float), 
    UnitGenerateOptions.Comparable | UnitGenerateOptions.Validate | UnitGenerateOptions.ArithmeticOperator | UnitGenerateOptions.JsonConverter)]
public readonly partial struct VolumeLevel
{
    private const float MinimumValue = 0f;
    private const float MaximumValue = 1.0f;

    public static readonly VolumeLevel Minimum = new(MinimumValue);
    public static readonly VolumeLevel Maximum = new(MaximumValue);

    private partial void Validate()
    {
        if (value < MinimumValue || MaximumValue < value)
        {
            throw new Exception("Invalid value range: " + value);
        }
    }
}