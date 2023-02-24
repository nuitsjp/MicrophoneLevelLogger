using MicrophoneLevelLogger.Client.Controller.Record;
using Sharprompt;

namespace MicrophoneLevelLogger.Client.View;

/// <summary>
/// 記録ビュー
/// </summary>
public class RecordView : MicrophoneView, IRecordView
{
    /// <summary>
    /// 記録名を入力する。
    /// </summary>
    /// <returns></returns>
    public string InputRecordName()
    {
        return Prompt.Input<string>("録音名を入力してください。");
    }

    /// <summary>
    /// 通知を開始する。
    /// </summary>
    /// <param name="timeSpan"></param>
    public void NotifyStarting(TimeSpan timeSpan)
    {
        ConsoleEx.WriteLine($"{timeSpan.Seconds}秒間、録音します。", ConsoleColor.White, ConsoleColor.Red);
    }
}