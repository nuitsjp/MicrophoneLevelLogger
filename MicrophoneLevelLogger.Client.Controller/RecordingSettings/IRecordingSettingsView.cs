namespace MicrophoneLevelLogger.Client.Controller.RecordingSettings;

public interface IRecordingSettingsView
{
    void ShowSettings(MicrophoneLevelLogger.RecordingSettings settings);

    bool ConfirmModify();
    int InputRecodingSpan();
    bool ConfirmEnableRemotePlaying();
    string InputMediaPlayerHost();
    bool ConfirmEnableRemoteRecording();
    string InputRecorderHost();
}