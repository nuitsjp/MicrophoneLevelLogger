namespace MicrophoneLevelLogger.Command;

public interface IMonitorVolumeView : IMicrophoneView
{
    void NotifyDetailMessage();
    /// <summary>
    /// 
    /// </summary>
    void WaitToBeStopped();
}