using Reactive.Bindings;

namespace Specter;

public interface IFastFourierTransformSettings
{
    ReactivePropertySlim<bool> EnableAWeighting { get; }
    ReactivePropertySlim<bool> EnableFastTimeWeighting { get; }
}