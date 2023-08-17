using Reactive.Bindings;

namespace Specter.Business;

public interface IDeviceInterface
{
    public ReadOnlyReactiveCollection<IDevice> Devices { get; }

    public Task ActivateAsync();
}