namespace MicrophoneLevelLogger.Domain;

public class MicrophoneCalibrationValue
{
    public MicrophoneCalibrationValue(
        string id,
        string name,
        MasterVolumeLevelScalar masterVolumeLevelScalar)
    {
        Id = id;
        Name = name;
        MasterVolumeLevelScalar = masterVolumeLevelScalar;
    }

    public string Id { get; }
    public string Name { get; }
    public MasterVolumeLevelScalar MasterVolumeLevelScalar { get; }
}