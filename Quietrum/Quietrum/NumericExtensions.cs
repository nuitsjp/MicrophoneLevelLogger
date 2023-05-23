namespace Quietrum;

/// <summary>
/// 数値関連拡張メソッド。
/// </summary>
public static class NumericExtensions
{
    /// <summary>
    /// 値が指定の範囲内に含まれるか確認する。
    /// </summary>
    /// <param name="value"></param>
    /// <param name="begin"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public static bool Between(this float value, float begin, float end)
    {
        if (value < begin) return false;
        return !(end < value);
    }
}