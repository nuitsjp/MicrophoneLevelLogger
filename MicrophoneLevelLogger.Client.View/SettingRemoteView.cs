using MicrophoneLevelLogger.Client.Controller.RecordingSettings;
using Sharprompt;

namespace MicrophoneLevelLogger.Client.View;

public class RecordingSettingsView : IRecordingSettingsView
{
    public void ShowSettings(RecordingSettings settings)
    {
        lock (this)
        {
            ConsoleEx.WriteLine();
            ConsoleEx.WriteLine($"録音時間[秒]     : {settings.RecordingSpan.Seconds}");
            ConsoleEx.WriteLine($"リモート録音実施 : {settings.IsEnableRemoteRecording}");
            ConsoleEx.WriteLine($"録音ホスト       : {settings.RecorderHost}");
            ConsoleEx.WriteLine($"リモート再生実施 : {settings.IsEnableRemotePlaying}");
            ConsoleEx.WriteLine($"音楽再生ホスト   : {settings.MediaPlayerHost}");
            ConsoleEx.WriteLine();
        }
    }

    public bool ConfirmModify()
    {
        return Prompt.Confirm("設定を変更しますか？", false);
    }

    public int InputRecodingSpan()
    {
        return Prompt.Input<int>("録音時間[秒]を入力してください。");
    }

    public bool ConfirmEnableRemotePlaying()
    {
        return Prompt.Confirm("リモート再生を有効にしますか？", false);
    }

    public string InputMediaPlayerHost()
    {
        return Prompt.Input<string>("音楽再生ホストを入力してください。");
    }

    public bool ConfirmEnableRemoteRecording()
    {
        return Prompt.Confirm("リモート録音を有効にしますか？", false);
    }

    public string InputRecorderHost()
    {
        return Prompt.Input<string>("録音ホストを入力してください。");
    }
}