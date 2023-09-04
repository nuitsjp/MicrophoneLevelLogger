using System.Reactive.Concurrency;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Reactive.Bindings;

namespace Specter.Business;

/// <summary>
/// マイク・スピーカーなどを統合したオーディオ関連のインターフェース
/// </summary>
public partial class AudioInterface : ObservableObject, IAudioInterface
{
    private readonly RemoteDeviceInterface _remoteDeviceInterface;

    private readonly LocalDeviceInterface _localDeviceInterface;

    public ReadOnlyReactiveCollection<IDevice> Devices { get; }

    /// <summary>
    /// すべてのマイクを扱うオーディオ インターフェースを作成する。
    /// </summary>
    /// <param name="localDeviceInterface"></param>
    /// <param name="remoteDeviceInterface"></param>
    internal AudioInterface(
        LocalDeviceInterface localDeviceInterface,
        RemoteDeviceInterface remoteDeviceInterface)
    {
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