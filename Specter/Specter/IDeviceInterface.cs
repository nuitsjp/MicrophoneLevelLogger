using Reactive.Bindings;

namespace Specter;

public interface IDeviceInterface
{
    public ReadOnlyReactiveCollection<IDevice> Devices { get; }

    public Task ActivateAsync();
}