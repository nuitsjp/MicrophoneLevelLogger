namespace MicrophoneLevelLogger;

/// <summary>
/// 帯域別の重み
/// </summary>
public readonly struct Weight
{
    /// <summary>
    /// 帯域
    /// </summary>
    public readonly Hz Frequency;
    /// <summary>
    /// 重み
    /// </summary>
    public readonly Decibel Value;

    /// <summary>
    /// インスタンスを生成する。
    /// </summary>
    /// <param name="frequency"></param>
    /// <param name="value"></param>
    public Weight(Hz frequency, Decibel value)
    {
        Frequency = frequency;
        Value = value;
    }
}
