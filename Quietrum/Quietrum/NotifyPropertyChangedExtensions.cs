using System.ComponentModel;
using System.Reactive.Linq;

namespace Quietrum;

public static class NotifyPropertyChangedExtensions
{
    public static IObservable<string> ToObservable(this INotifyPropertyChanged source)
    {
        return Observable.FromEvent<PropertyChangedEventHandler, string>(
            h => (sender, e) => h(e.PropertyName!),
            h => source.PropertyChanged += h,
            h => source.PropertyChanged -= h);
    }
}