namespace MicrophoneLevelLogger;

public class AudioInterfaceProvider : IAudioInterfaceProvider
{
    private readonly ISettingsRepository _settingsRepository;

    public AudioInterfaceProvider(ISettingsRepository settingsRepository)
    {
        _settingsRepository = settingsRepository;
    }

    public IAudioInterface Resolve()
    {
        return new AudioInterface(_settingsRepository);
    }
}