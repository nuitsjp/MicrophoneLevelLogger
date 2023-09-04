using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Reactive.Bindings;

namespace Specter.Business;

/// <summary>
/// マイク・スピーカーなどを統合したオーディオ関連のインターフェース
/// </summary>
public interface IAudioInterface : INotifyPropertyChanged, IDisposable
{
    Task ActivateAsync();
    
    /// <summary>
    /// 
    /// </summary>
    ReadOnlyReactiveCollection<IDevice> Devices { get; }
}