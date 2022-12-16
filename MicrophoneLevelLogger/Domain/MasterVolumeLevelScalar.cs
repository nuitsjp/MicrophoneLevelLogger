using UnitGenerator;

namespace MicrophoneLevelLogger.Domain;

[UnitOf(typeof(float), UnitGenerateOptions.Comparable | UnitGenerateOptions.Validate | UnitGenerateOptions.ArithmeticOperator)]
public readonly partial struct MasterVolumeLevelScalar
{
    private const float MinimumValue = 0f;
    private const float MaximumValue = 1.0f;

    public static readonly MasterVolumeLevelScalar Minimum = new(MinimumValue);
    public static readonly MasterVolumeLevelScalar Maximum = new(MaximumValue);

    private partial void Validate()
    {
        if (value < MinimumValue || MaximumValue < value)
        {
            throw new Exception("Invalid value range: " + value);
        }
    }
}