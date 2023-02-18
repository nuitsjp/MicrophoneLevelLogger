namespace MicrophoneLevelLogger;

public class RecordSummary
{
    private readonly List<MicrophoneRecordSummary> _microphones;

    public RecordSummary(
        string name,
        DateTime begin, 
        DateTime end,
        List<MicrophoneRecordSummary> microphones)
    {
        Name = name;
        Begin = begin;
        End = end;
        _microphones = microphones.ToList();
    }

    public string Name { get; }
    public DateTime Begin { get; }
    public DateTime End { get; }
    public List<MicrophoneRecordSummary> Microphones => _microphones;
}