using MicrophoneLevelLogger.Client.Controller.MonitorVolume;

namespace MicrophoneLevelLogger.Client.View;

public class MonitorVolumeView : MicrophoneView, IMonitorVolumeView
{
    public void NotifyDetailMessage()
    {
        ConsoleEx.WriteLine("Enterキーでモニターを停止します。");
        ConsoleEx.WriteLine();
    }

    public void WaitToBeStopped()
    {
        Console.ReadLine();
    }
}