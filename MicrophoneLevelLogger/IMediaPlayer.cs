namespace MicrophoneLevelLogger;

/// <summary>
/// 音源を再生するプレイヤー
/// </summary>
public interface IMediaPlayer
{
    /// <summary>
    /// キャンセルされるまでループ再生する。
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    Task PlayLoopingAsync(CancellationToken token);
}