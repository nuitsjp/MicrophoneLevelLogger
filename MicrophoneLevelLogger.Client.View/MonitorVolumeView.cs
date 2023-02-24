using MicrophoneLevelLogger.Client.Controller.MonitorVolume;

namespace MicrophoneLevelLogger.Client.View;

public class MonitorVolumeView : MicrophoneView, IMonitorVolumeView
{
    public void WaitToBeStopped()
    {
        // ReSharper disable once LocalizableElement
        Console.WriteLine("Enterキーでモニターを停止します。");
        Console.WriteLine();

        Console.ReadLine();
    }
}