namespace MicrophoneLevelLogger;

/// <summary>
/// マイク別のサンプリング間隔ごとの最大音量を記録するライター
/// </summary>
public interface IDetailRepository : IDisposable
{
    /// <summary>
    /// ヘッダーを記録する。
    /// </summary>
    /// <param name="recorders"></param>
    /// <returns></returns>
    Task WriteHeaderAsync(IEnumerable<IMicrophoneRecorder> recorders);
    /// <summary>
    /// サンプリング結果を記録する。
    /// </summary>
    /// <param name="recorders"></param>
    /// <returns></returns>
    Task WriteRecordAsync(IEnumerable<IMicrophoneRecorder> recorders);
}