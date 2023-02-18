namespace MicrophoneLevelLogger.Client.Controller;

public interface IMicrophoneView
{
    void NotifyMicrophonesInformation(IAudioInterface audioInterface);
    void StartNotify(IRecorder recorder, CancellationToken token);
    public void NotifyResult(IRecorder logger);
    void Wait(TimeSpan timeSpan);
}