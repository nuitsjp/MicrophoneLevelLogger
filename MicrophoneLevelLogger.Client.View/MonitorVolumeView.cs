using MicrophoneLevelLogger.Client.Command.MonitorVolume;

namespace MicrophoneLevelLogger.Client.View;

public class MonitorVolumeView : MicrophoneView, IMonitorVolumeView
{
    public void NotifyDetailMessage()
    {
        Console.WriteLine("Enterキーでモニターを停止します。");
        Console.WriteLine();
    }

    public void WaitToBeStopped()
    {
        Console.ReadLine();
    }
}