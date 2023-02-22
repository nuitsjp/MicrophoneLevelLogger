namespace MicrophoneLevelLogger.Client.Controller;

public interface IMicrophoneView
{
    Task NotifyAudioInterfaceAsync(IAudioInterface audioInterface);
    void StartNotify(IRecorder recorder, CancellationToken token);
    public void NotifyResult(IRecorder logger);
    void Wait(TimeSpan timeSpan);
}