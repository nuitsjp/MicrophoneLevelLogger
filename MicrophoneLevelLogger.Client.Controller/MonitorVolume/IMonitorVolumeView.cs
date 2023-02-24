namespace MicrophoneLevelLogger.Client.Controller.MonitorVolume;

/// <summary>
/// マイクの入力モニタービュー。
/// </summary>
public interface IMonitorVolumeView : IMicrophoneView
{
    /// <summary>
    /// モニター終了を待機する。
    /// </summary>
    void WaitToBeStopped();
}