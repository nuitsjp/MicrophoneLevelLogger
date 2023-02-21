namespace MicrophoneLevelLogger;

public interface ISettingsRepository
{
    Task<Settings> LoadAsync();
    Settings Load();
    Task SaveAsync(Settings settings);
}