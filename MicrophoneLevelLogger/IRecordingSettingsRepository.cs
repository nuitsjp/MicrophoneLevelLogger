namespace MicrophoneLevelLogger;

public interface IRecordingSettingsRepository
{
    Task<RecordingSettings> LoadAsync();
    Task SaveAsync(RecordingSettings settings);
}