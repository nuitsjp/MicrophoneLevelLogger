namespace MicrophoneLevelLogger;

public class MicrophoneCalibrationValue
{
    public MicrophoneCalibrationValue(
        string id,
        string name,
        VolumeLevel volumeLevel)
    {
        Id = id;
        Name = name;
        VolumeLevel = volumeLevel;
    }

    public string Id { get; }
    public string Name { get; }
    public VolumeLevel VolumeLevel { get; }
}