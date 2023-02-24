namespace MicrophoneLevelLogger.Client.Controller.Record;

/// <summary>
/// 記録ビュー
/// </summary>
public interface IRecordView : IMicrophoneView
{
    /// <summary>
    /// 記録名を入力する。
    /// </summary>
    /// <returns></returns>
    public string InputRecordName();
    /// <summary>
    /// 通知を開始する。
    /// </summary>
    /// <param name="timeSpan"></param>
    public void NotifyStarting(TimeSpan timeSpan);
}