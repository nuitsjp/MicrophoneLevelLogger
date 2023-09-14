using Reactive.Bindings;

namespace Specter;

public class FastFourierTransformSettings : IFastFourierTransformSettings
{
    public ReactivePropertySlim<bool> EnableAWeighting { get; } = new(true);
    public ReactivePropertySlim<bool> EnableFastTimeWeighting { get; } = new(true);
}