using System.Threading.Tasks;

namespace Specter.Business;

/// <summary>
/// IAudioInterfaceのプロバイダー
/// </summary>
public class AudioInterfaceProvider : IAudioInterfaceProvider
{
    /// <summary>
    /// ISettingsRepository
    /// </summary>
    private readonly ISettingsRepository _settingsRepository;

    private IAudioInterface? _audioInterface;

    /// <summary>
    /// インスタンスを生成する。
    /// </summary>
    /// <param name="settingsRepository"></param>
    public AudioInterfaceProvider(
        ISettingsRepository settingsRepository)
    {
        _settingsRepository = settingsRepository;
    }

    /// <summary>
    /// IAudioInterfaceを解決する。
    /// </summary>
    /// <returns></returns>
    public IAudioInterface Resolve()
    {
        return _audioInterface ??= 
            new AudioInterface(
                new LocalDeviceInterface(_settingsRepository),
                new RemoteDeviceInterface());
    }
}