namespace MicrophoneLevelLogger;

public static class EnumerableExtension
{
    public static bool Empty<TSource>(this IEnumerable<TSource> source)
    {
        return source.Any() is false;
    }

    public static bool NotContains<TSource>(this IEnumerable<TSource> source, TSource value)
    {
        return source.Contains(value) is false;
    }

    public static void Remove<TSource>(this IList<TSource> enumerable, Func<TSource, bool> func)
    {
        enumerable
            .Where(func)
            .ToList()
            .ForEach(x => enumerable.Remove(x));
    }

    public static async Task ForEachAsync<T>(this IEnumerable<T> enumerable, Func<T, Task> action)
    {
        await Parallel.ForEachAsync(
            enumerable,
            async (item, _) => await action(item));
    }
}

public static class NumericExtensions
{
    public static bool Between(this float value, float begin, float end)
    {
        if (value < begin) return false;
        return !(end < value);
    }
}