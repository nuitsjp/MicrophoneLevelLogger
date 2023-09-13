using System.Reactive.Linq;
using System.Reactive.Subjects;
using Reactive.Bindings;

namespace Specter;

public class FastFourierTransformSettings : IFastFourierTransformSettings
{
    public ReactivePropertySlim<bool> EnableAWeighting { get; } = new(true);
    public ReactivePropertySlim<bool> EnableFastTimeWeighting { get; } = new(true);
}