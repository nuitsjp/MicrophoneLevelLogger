namespace LogAggregator;

public class CsvSummary
{
    public CsvSummary(int no, string name, double min, double avg, double median, double max)
    {
        No = no;
        Name = name;
        Min = min;
        Avg = avg;
        Median = median;
        Max = max;
    }

    public int No { get; }
    public string Name { get; }
    public double Min { get; }
    public double Avg { get; }
    public double Median { get; }
    public double Max { get; }
}