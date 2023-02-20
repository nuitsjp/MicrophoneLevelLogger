namespace MicrophoneLevelLogger;

public interface ISettingsRepository
{
    Task<Settings> LoadAsync();
    Task SaveAsync(Settings settings);
}