using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Quietrum;

public static class ReactiveCollectionExtensions
{
    public static ReactiveCollection<T> Merge<T>(this ReadOnlyReactiveCollection<T> first, ReadOnlyReactiveCollection<T> second)
    {
        var mergedCollection = new ReactiveCollection<T>();

        foreach(var item in first)
            mergedCollection.Add(item);

        foreach(var item in second)
            mergedCollection.Add(item);

        first.ObserveAddChanged()
            .ObserveOn(CurrentThreadScheduler.Instance)
            .Subscribe(item => mergedCollection.Add(item));

        second.ObserveAddChanged()
            .ObserveOn(CurrentThreadScheduler.Instance)
            .Subscribe(item => mergedCollection.Add(item));

        first.ObserveRemoveChanged()
            .ObserveOn(CurrentThreadScheduler.Instance)
            .Subscribe(item => mergedCollection.Remove(item));

        second.ObserveRemoveChanged()
            .ObserveOn(CurrentThreadScheduler.Instance)
            .Subscribe(item => mergedCollection.Remove(item));

        return mergedCollection;
    }
}
