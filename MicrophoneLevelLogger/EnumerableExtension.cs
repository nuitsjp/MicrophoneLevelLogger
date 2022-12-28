namespace MicrophoneLevelLogger;

public static class EnumerableExtension
{
    public static double Median(this IEnumerable<double>? source)
    {
        // ReSharper disable once PossibleMultipleEnumeration
        if (source is null || !source.Any())
        {
            throw new InvalidOperationException("Cannot compute median for a null or empty set.");
        }

        var sortedList =
            // ReSharper disable once PossibleMultipleEnumeration
            source.OrderBy(number => number).ToList();

        int itemIndex = sortedList.Count / 2;

        if (sortedList.Count % 2 == 0)
        {
            // Even number of items.
            return (sortedList[itemIndex] + sortedList[itemIndex - 1]) / 2;
        }
        else
        {
            // Odd number of items.
            return sortedList[itemIndex];
        }
    }
}