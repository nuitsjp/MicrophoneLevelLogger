using FluentTextTable;
using MicrophoneLevelLogger.Client.Controller.Record;
using Sharprompt;

namespace MicrophoneLevelLogger.Client.View;

public class RecordView : MicrophoneView, IRecordView
{
    public string InputRecordName()
    {
        return Prompt.Input<string>("録音名を入力してください。");
    }

    public void NotifyStarting(TimeSpan timeSpan)
    {
        ConsoleEx.WriteLine($"{timeSpan.Seconds}秒間、録音します。", ConsoleColor.White, ConsoleColor.Red);
    }
}