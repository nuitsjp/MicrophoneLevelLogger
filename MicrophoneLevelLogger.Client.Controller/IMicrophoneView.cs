namespace MicrophoneLevelLogger.Client.Controller;

public interface IMicrophoneView
{
    void NotifyMicrophonesInformation(IAudioInterface audioInterface);
    void StartNotify(IAudioInterfaceLogger audioInterfaceLogger, CancellationToken token);
    public void NotifyResult(IAudioInterfaceLogger logger);
    void Wait(TimeSpan timeSpan);
}