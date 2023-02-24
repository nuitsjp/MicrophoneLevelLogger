using MicrophoneLevelLogger.Client.Controller.RecordingSettings;
using Sharprompt;

namespace MicrophoneLevelLogger.Client.View;

/// <summary>
/// 記録設定確認・変更ビュー
/// </summary>
public class RecordingSettingsView : IRecordingSettingsView
{
    /// <summary>
    /// 現在の設定を表示する。
    /// </summary>
    /// <param name="settings"></param>
    public void ShowSettings(Settings settings)
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

    /// <summary>
    /// 変更を確認する。
    /// </summary>
    /// <returns></returns>
    public bool ConfirmModify()
    {
        return Prompt.Confirm("設定を変更しますか？", false);
    }

    /// <summary>
    /// 記録時間を入力する。
    /// </summary>
    /// <returns></returns>
    public int InputRecodingSpan()
    {
        return Prompt.Input<int>("録音時間[秒]を入力してください。", 30);
    }

    /// <summary>
    /// リモート再生の実施可否を確認する。
    /// </summary>
    /// <returns></returns>
    public bool ConfirmEnableRemotePlaying()
    {
        return Prompt.Confirm("リモート再生を有効にしますか？", false);
    }

    /// <summary>
    /// リモートメディアプレイヤーのホスト名を入力する。
    /// </summary>
    /// <returns></returns>
    public string InputMediaPlayerHost()
    {
        return Prompt.Input<string>("音楽再生ホストを入力してください。");
    }

    /// <summary>
    /// リモート記録の実施可否を確認する。
    /// </summary>
    /// <returns></returns>
    public bool ConfirmEnableRemoteRecording()
    {
        return Prompt.Confirm("リモート録音を有効にしますか？", false);
    }

    /// <summary>
    /// リモートレコーダーのホスト名を入力する。
    /// </summary>
    /// <returns></returns>
    public string InputRecorderHost()
    {
        return Prompt.Input<string>("録音ホストを入力してください。");
    }
}