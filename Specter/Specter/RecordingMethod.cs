namespace Specter;

public class RecordingMethod
{
    private static readonly List<RecordingMethod> Methods = 
        new RecordingMethod[]
        {
            new("Front with Playback", true),
            new("Rear with Playback", true),
            new("Left with Playback", true),
            new("Right with Playback", true),
        }.ToList();

    public static IReadOnlyList<RecordingMethod> RecordingMethods => Methods;
    
    // ReSharper disable once MemberCanBePrivate.Global
    public RecordingMethod(string name, bool withPlayback)
    {
        Name = name;
        WithPlayback = withPlayback;
    }

    public string Name { get; }
    
    public bool WithPlayback { get; }

    public override string ToString() => Name;
}