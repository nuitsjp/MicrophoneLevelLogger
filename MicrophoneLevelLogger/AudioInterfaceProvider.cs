namespace MicrophoneLevelLogger;

public class AudioInterfaceProvider : IAudioInterfaceProvider
{
    private readonly ISettingsRepository _settingsRepository;

    public AudioInterfaceProvider(ISettingsRepository settingsRepository)
    {
        _settingsRepository = settingsRepository;
    }

    public async Task<IAudioInterface> ResolveAsync()
    {
        var settings = await _settingsRepository.LoadAsync();
        return new AudioInterface(settings);
    }
}