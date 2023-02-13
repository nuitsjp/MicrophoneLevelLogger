namespace MicrophoneLevelLogger.Client.Command.RecordingSettings;

public interface IRecordingSettingsView
{
    void ShowSettings(Domain.RecordingSettings settings);

    bool ConfirmModify();
    int InputRecodingSpan();
    bool ConfirmEnableRemotePlaying();
    string InputMediaPlayerHost();
    bool ConfirmEnableRemoteRecording();
    string InputRecorderHost();
}