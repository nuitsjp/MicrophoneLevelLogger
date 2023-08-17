using System.Collections.ObjectModel;
using Reactive.Bindings;

namespace Quietrum;

public interface IDeviceInterface
{
    public ReadOnlyReactiveCollection<IDevice> Devices { get; }

    public Task ActivateAsync();
}