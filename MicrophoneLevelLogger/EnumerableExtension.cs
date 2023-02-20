﻿using Microsoft.AspNetCore.Http.Features;

namespace MicrophoneLevelLogger;

public static class EnumerableExtension
{
    public static bool Empty<TSource>(this IEnumerable<TSource> sources)
    {
        return sources.Any() is false;
    }

    public static void Remove<TSource>(this IList<TSource> enumerable, Func<TSource, bool> func)
    {
        enumerable
            .Where(func)
            .ToList()
            .ForEach(x => enumerable.Remove(x));
    }

    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
        foreach (var item in enumerable)
        {
            action(item);
        }
    }
    public static async Task ForEachAsync<T>(this IEnumerable<T> enumerable, Func<T, Task> action)
    {
        await Parallel.ForEachAsync(
            enumerable,
            async (item, _) => await action(item));
    }
}