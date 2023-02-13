namespace MicrophoneLevelLogger;

public interface IAudioInterfaceCalibrationValuesRepository
{
    Task<AudioInterfaceCalibrationValues> LoadAsync();
    Task SaveAsync(AudioInterfaceCalibrationValues audioInterfaceCalibrationValues);
    void Remove();
}