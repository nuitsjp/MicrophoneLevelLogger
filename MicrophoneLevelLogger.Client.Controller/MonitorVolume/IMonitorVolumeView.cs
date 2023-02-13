namespace MicrophoneLevelLogger.Client.Command.MonitorVolume;

public interface IMonitorVolumeView : IMicrophoneView
{
    void NotifyDetailMessage();
    /// <summary>
    /// 
    /// </summary>
    void WaitToBeStopped();
}