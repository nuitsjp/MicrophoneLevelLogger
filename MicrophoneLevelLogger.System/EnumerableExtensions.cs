namespace MicrophoneLevelLogger;

/// <summary>
/// IEnumerable拡張メソッド
/// </summary>
public static class EnumerableExtension
{
    /// <summary>
    /// IEnumerableが空かどうか判定する。
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static bool Empty<TSource>(this IEnumerable<TSource> source)
    {
        return source.Any() is false;
    }

    /// <summary>
    /// 指定要素を含んでいるか確認する。
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool NotContains<TSource>(this IEnumerable<TSource> source, TSource value)
    {
        return source.Contains(value) is false;
    }

    /// <summary>
    /// 指定の要素を削除する。
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="enumerable"></param>
    /// <param name="func"></param>
    public static void Remove<TSource>(this IList<TSource> enumerable, Func<TSource, bool> func)
    {
        enumerable
            .Where(func)
            .ToList()
            .ForEach(x => enumerable.Remove(x));
    }

    /// <summary>
    /// 非同期繰り返し処理を実行する。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumerable"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static async Task ForEachAsync<T>(this IEnumerable<T> enumerable, Func<T, Task> action)
    {
        await Parallel.ForEachAsync(
            enumerable,
            async (item, _) => await action(item));
    }
}