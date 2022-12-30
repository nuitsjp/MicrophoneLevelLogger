namespace LogAggregator;

public class CsvSummary
{
    public int No { get; set; }
    public string Name { get; set; }
    public double Min { get; set; }
    public double Avg { get; set; }
    public double Median { get; set; }
    public double Max { get; set; }
}

public class Summary
{
    public string Name { get; set; }
    public Direction Direction { get; set; }
    public double Min { get; set; }
    public double Avg { get; set; }
    public double Median { get; set; }
    public double Max { get; set; }

}

public enum Direction
{
    Front,
    Right,
    Left,
    Back
}