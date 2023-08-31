using System.Reactive.Concurrency;
using CommunityToolkit.Mvvm.ComponentModel;
using Reactive.Bindings;

namespace Specter.Business;

/// <summary>
/// マイク・スピーカーなどを統合したオーディオ関連のインターフェース
/// </summary>
public partial class AudioInterface : ObservableObject, IAudioInterface
{
    private readonly ISettingsRepository _settingsRepository;

    private readonly RemoteDeviceInterface _remoteDeviceInterface;

    private readonly LocalDeviceInterface _localDeviceInterface;

    public ReadOnlyReactiveCollection<IDevice> Devices { get; }

    /// <summary>
    /// すべてのマイクを扱うオーディオ インターフェースを作成する。
    /// </summary>
    /// <param name="settingsRepository"></param>
    /// <param name="localDeviceInterface"></param>
    /// <param name="remoteDeviceInterface"></param>
    internal AudioInterface(
        ISettingsRepository settingsRepository, 
        LocalDeviceInterface localDeviceInterface,
        RemoteDeviceInterface remoteDeviceInterface)
    {
        _settingsRepository = settingsRepository;
        _localDeviceInterface = localDeviceInterface;
        _remoteDeviceInterface = remoteDeviceInterface;
        Devices = _localDeviceInterface
            .Devices
            .Merge(_remoteDeviceInterface.Devices)
            .ToReadOnlyReactiveCollection(scheduler: CurrentThreadScheduler.Instance);

    }

    public async Task ActivateAsync()
    {
        await _localDeviceInterface.ActivateAsync();
        await _remoteDeviceInterface.ActivateAsync();
    }

    public void Dispose()
    {
        Devices.Dispose();
    }
}