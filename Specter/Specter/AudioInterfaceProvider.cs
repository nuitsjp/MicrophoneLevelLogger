namespace Specter;

/// <summary>
/// IAudioInterfaceのプロバイダー
/// </summary>
public class AudioInterfaceProvider : IAudioInterfaceProvider
{
    /// <summary>
    /// ISettingsRepository
    /// </summary>
    private readonly ISettingsRepository _settingsRepository;

    private readonly IFastFourierTransformSettings _fastFourierTransformSettings;

    private IAudioInterface? _audioInterface;

    /// <summary>
    /// インスタンスを生成する。
    /// </summary>
    /// <param name="settingsRepository"></param>
    public AudioInterfaceProvider(
        ISettingsRepository settingsRepository, 
        IFastFourierTransformSettings fastFourierTransformSettings)
    {
        _settingsRepository = settingsRepository;
        _fastFourierTransformSettings = fastFourierTransformSettings;
    }

    /// <summary>
    /// IAudioInterfaceを解決する。
    /// </summary>
    /// <returns></returns>
    public IAudioInterface Resolve()
    {
        return _audioInterface ??= 
            new AudioInterface(
                new LocalDeviceInterface(_settingsRepository, _fastFourierTransformSettings),
                new RemoteDeviceInterface(_fastFourierTransformSettings));
    }
}