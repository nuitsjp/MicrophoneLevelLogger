namespace LogAggregator;

public class Summary
{
    public Summary(string name, Direction direction, double min, double avg, double median, double max)
    {
        Name = name;
        Direction = direction;
        Min = min;
        Avg = avg;
        Median = median;
        Max = max;
    }

    public string Name { get; }
    public Direction Direction { get; }
    public double Min { get; }
    public double Avg { get; }
    public double Median { get; }
    public double Max { get; }

}