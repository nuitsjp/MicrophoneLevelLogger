namespace MicrophoneLevelLogger;

public class RecordSummary
{
    private readonly List<MicrophoneRecordSummary> _microphones;

    public RecordSummary(
        string name,
        DateTime begin, 
        DateTime end, 
        IEnumerable<MicrophoneRecordSummary> microphones)
    {
        Name = name;
        Begin = begin;
        End = end;
        _microphones = microphones.ToList();
    }

    public string Name { get; }
    public DateTime Begin { get; }
    public DateTime End { get; }
    public IReadOnlyList<MicrophoneRecordSummary> Microphones => _microphones;
}