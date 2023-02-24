using MicrophoneLevelLogger.Client.Controller.MonitorVolume;

namespace MicrophoneLevelLogger.Client.View;

/// <summary>
/// マイクの入力モニタービュー。
/// </summary>
public class MonitorVolumeView : MicrophoneView, IMonitorVolumeView
{
    /// <summary>
    /// モニター終了を待機する。
    /// </summary>
    public void WaitToBeStopped()
    {
        // ReSharper disable once LocalizableElement
        Console.WriteLine("Enterキーでモニターを停止します。");
        Console.WriteLine();

        Console.ReadLine();
    }
}