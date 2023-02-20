namespace MicrophoneLevelLogger;

public class MicrophoneCalibrationValue
{
    public MicrophoneCalibrationValue(
        MicrophoneId id,
        string name,
        VolumeLevel volumeLevel)
    {
        Id = id;
        Name = name;
        VolumeLevel = volumeLevel;
    }

    public MicrophoneId Id { get; }
    public string Name { get; }
    public VolumeLevel VolumeLevel { get; }
}