namespace MicrophoneLevelLogger.Client.Controller;

/// <summary>
/// 汎用ビュー
/// </summary>
public interface IMicrophoneView
{
    /// <summary>
    /// オーディオインターフェースの状態を通知する。
    /// </summary>
    /// <param name="audioInterface"></param>
    /// <returns></returns>
    Task NotifyAudioInterfaceAsync(IAudioInterface audioInterface);
    /// <summary>
    /// レコーダーの記録状況を通知する。
    /// </summary>
    /// <param name="recorder"></param>
    /// <param name="token"></param>
    void StartNotify(IRecorder recorder, CancellationToken token);
    /// <summary>
    /// 記録結果を通知する。
    /// </summary>
    /// <param name="logger"></param>
    public void NotifyResult(IRecorder logger);
    /// <summary>
    /// Enterでキャンセルできる状態で、指定時間待機する。
    /// </summary>
    /// <param name="timeSpan"></param>
    void Wait(TimeSpan timeSpan);
}