namespace MicrophoneLevelLogger.Command.MonitorVolume;

public interface IMonitorVolumeView : IMicrophoneView
{
    void NotifyDetailMessage();
    /// <summary>
    /// 
    /// </summary>
    void WaitToBeStopped();
}