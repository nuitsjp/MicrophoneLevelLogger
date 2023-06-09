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

    private readonly IRemoteDeviceServer _remoteDeviceServer;

    /// <summary>
    /// インスタンスを生成する。
    /// </summary>
    /// <param name="settingsRepository"></param>
    /// <param name="remoteDeviceServer"></param>
    public AudioInterfaceProvider(
        ISettingsRepository settingsRepository, 
        IRemoteDeviceServer remoteDeviceServer)
    {
        _settingsRepository = settingsRepository;
        _remoteDeviceServer = remoteDeviceServer;
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
                _remoteDeviceServer,
                new LocalDeviceInterface(_settingsRepository));
        await audioInterface.ActivateAsync();
        return audioInterface;
    }
}