using System.ComponentModel;

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
    IReadOnlyList<IMicrophone> Microphones { get; }

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