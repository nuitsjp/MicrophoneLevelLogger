namespace Quietrum;

/// <summary>
/// マイクのレコーダー
/// </summary>
public interface IMicrophoneRecorder
{
    /// <summary>
    /// 録音対象のマイク
    /// </summary>
    public IMicrophone Microphone { get; }
    /// <summary>
    /// 最大音量
    /// </summary>
    public Decibel Max { get; }
    /// <summary>
    /// 平均音量
    /// </summary>
    public Decibel Avg { get; }
    /// <summary>
    /// 最小音量
    /// </summary>
    public Decibel Min { get; }
    /// <summary>
    /// 録音を開始する。
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task StartAsync(CancellationToken token);
}