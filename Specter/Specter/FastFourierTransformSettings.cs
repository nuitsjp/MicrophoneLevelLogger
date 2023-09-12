using System.Reactive.Linq;
using System.Reactive.Subjects;
using Reactive.Bindings;

namespace Specter;

public static class FastFourierTransformSettings
{
    public static ReactivePropertySlim<bool> EnableAWeighting { get; } = new(true);
    public static ReactivePropertySlim<bool> EnableFastTimeWeighting { get; } = new(true);
}