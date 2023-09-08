namespace Specter;

/// <summary>
/// 周波数帯域別の音量
/// </summary>
public readonly struct DecibelByFrequency
{
    /// <summary>
    /// 周波数
    /// </summary>
    public readonly Hz Frequency;
    /// <summary>
    /// 音量
    /// </summary>
    public readonly Decibel Decibel;

    /// <summary>
    /// インスタンスを生成する
    /// </summary>
    /// <param name="frequency"></param>
    /// <param name="decibel"></param>
    public DecibelByFrequency(Hz frequency, Decibel decibel)
    {
        Frequency = frequency;
        Decibel = decibel;
    }

    /// <summary>
    /// 文字列表現を取得する。
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Frequency={Frequency}, Decibel={Decibel}";
}