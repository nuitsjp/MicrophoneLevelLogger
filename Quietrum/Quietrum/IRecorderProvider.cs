namespace Quietrum;

/// <summary>
/// レコーダーのプロバイダー
/// </summary>
public interface IRecorderProvider
{
    /// <summary>
    /// ローカルのレコーダーを解決する。
    /// </summary>
    /// <param name="audioInterface"></param>
    /// <param name="recordName">nullの場合、計測はするが録音をローカルに保存しない。</param>
    /// <returns></returns>
    IRecorder ResolveLocal(IAudioInterface audioInterface, string? recordName);
    /// <summary>
    /// 指定されたマイクを解決する。
    /// </summary>
    /// <param name="microphones"></param>
    /// <returns></returns>
    IRecorder ResolveLocal(params IMicrophone[] microphones);
    /// <summary>
    /// リモートのレコーダーを解決する。
    /// </summary>
    /// <param name="recordName"></param>
    /// <returns></returns>
    IRecorder ResolveRemote(string? recordName);
}