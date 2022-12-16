namespace MicrophoneLevelLogger.View;

public interface IMonitorVolumeView : IMicrophoneView
{
    void NotifyDetailMessage();
    /// <summary>
    /// 
    /// </summary>
    void WaitToBeStopped();
}