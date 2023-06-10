using System.Collections.ObjectModel;
using Reactive.Bindings;

namespace Quietrum;

public interface IDeviceInterface<TDevice> where TDevice : IDevice
{
    public ReadOnlyReactiveCollection<TDevice> Devices { get; }

    public Task ActivateAsync();
}