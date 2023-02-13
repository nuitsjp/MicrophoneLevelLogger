namespace MicrophoneLevelLogger.Client.Controller.MonitorVolume;

public interface IMonitorVolumeView : IMicrophoneView
{
    void NotifyDetailMessage();
    /// <summary>
    /// 
    /// </summary>
    void WaitToBeStopped();
}