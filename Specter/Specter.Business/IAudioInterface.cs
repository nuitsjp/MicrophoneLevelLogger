using System.Collections.ObjectModel;
using System.ComponentModel;
using Reactive.Bindings;

namespace Quietrum;

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

    /// <summary>
    /// 現在有効なスピーカーを取得する。
    /// </summary>
    /// <returns></returns>
    Task<ISpeaker> GetSpeakerAsync();

    /// <summary>
    /// すべてのスピーカーを取得する。
    /// </summary>
    /// <returns></returns>
    IEnumerable<ISpeaker> GetSpeakers();
}