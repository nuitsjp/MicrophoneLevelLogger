using LogAggregator;

public class MicrophoneRecord
{
    public MicrophoneRecord(string productName, Direction direction)
    {
        ProductName = productName;
        Direction = direction;
    }

    public string ProductName { get; }
    public Direction Direction { get; }
    public List<double> Decibels { get; } = new();
}