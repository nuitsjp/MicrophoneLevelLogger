namespace MicrophoneLevelLogger;

/// <summary>
/// マイク・スピーカーなどを統合したオーディオ関連のインターフェース
/// </summary>
public interface IAudioInterface
{
    /// <summary>
    /// マイク一覧を取得する。
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    IEnumerable<IMicrophone> GetMicrophones(MicrophoneStatus status = MicrophoneStatus.Enable);

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