namespace Quietrum;

/// <summary>
/// IAudioInterfaceのプロバイダー
/// </summary>
public class AudioInterfaceProvider : IAudioInterfaceProvider
{
    /// <summary>
    /// ISettingsRepository
    /// </summary>
    private readonly ISettingsRepository _settingsRepository;

    /// <summary>
    /// インスタンスを生成する。
    /// </summary>
    /// <param name="settingsRepository"></param>
    /// <param name="remoteDeviceServer"></param>
    public AudioInterfaceProvider(
        ISettingsRepository settingsRepository)
    {
        _settingsRepository = settingsRepository;
    }

    /// <summary>
    /// IAudioInterfaceを解決する。
    /// </summary>
    /// <returns></returns>
    public async Task<IAudioInterface> ResolveAsync()
    {
        var audioInterface = 
            new AudioInterface(
                _settingsRepository, 
                new LocalDeviceInterface(_settingsRepository),
                new RemoteDeviceInterface());
        return audioInterface;
    }
}