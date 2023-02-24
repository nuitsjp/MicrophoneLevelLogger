namespace MicrophoneLevelLogger.Client.Controller.RecordingSettings;

/// <summary>
/// 記録設定確認・変更ビュー
/// </summary>
public interface IRecordingSettingsView
{
    /// <summary>
    /// 現在の設定を表示する。
    /// </summary>
    /// <param name="settings"></param>
    void ShowSettings(Settings settings);
    /// <summary>
    /// 変更を確認する。
    /// </summary>
    /// <returns></returns>
    bool ConfirmModify();
    /// <summary>
    /// 記録時間を入力する。
    /// </summary>
    /// <returns></returns>
    int InputRecodingSpan();
    /// <summary>
    /// リモート再生の実施可否を確認する。
    /// </summary>
    /// <returns></returns>
    bool ConfirmEnableRemotePlaying();
    /// <summary>
    /// リモートメディアプレイヤーのホスト名を入力する。
    /// </summary>
    /// <returns></returns>
    string InputMediaPlayerHost();
    /// <summary>
    /// リモート記録の実施可否を確認する。
    /// </summary>
    /// <returns></returns>
    bool ConfirmEnableRemoteRecording();
    /// <summary>
    /// リモートレコーダーのホスト名を入力する。
    /// </summary>
    /// <returns></returns>
    string InputRecorderHost();
}