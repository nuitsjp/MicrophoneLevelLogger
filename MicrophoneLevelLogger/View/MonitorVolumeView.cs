namespace MicrophoneLevelLogger.View;

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