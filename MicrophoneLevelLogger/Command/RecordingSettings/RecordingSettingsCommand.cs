using MicrophoneLevelLogger.Domain;
using Sharprompt;

namespace MicrophoneLevelLogger.Command.RecordingSettings;

public class RecordingSettingsCommand : ICommand
{
    private IRecordingSettingsView _view;

    public RecordingSettingsCommand(IRecordingSettingsView view)
    {
        _view = view;
    }

    public string Name => "Recoding Settings    : 録音設定を確認・変更する。";

    public async Task ExecuteAsync()
    {
        var settings = await Domain.RecordingSettings.LoadAsync();
        _view.ShowSettings(settings);

        if (_view.ConfirmModify())
        {
            var recordingSpan = _view.InputRecodingSpan();
            var isEnableRemoteRecording = _view.ConfirmEnableRemoteRecording();
            var recorderHost = isEnableRemoteRecording
                ? _view.InputRecorderHost()
                : "localhost";
            var isEnableRemotePlaying = _view.ConfirmEnableRemotePlaying();
            var mediaPlayerHost = isEnableRemotePlaying
                ? _view.InputMediaPlayerHost()
                : "localhost";

            await Domain.RecordingSettings.SaveAsync(
                new Domain.RecordingSettings(
                    mediaPlayerHost,
                    recorderHost,
                    TimeSpan.FromSeconds(recordingSpan),
                    isEnableRemotePlaying,
                    isEnableRemoteRecording));

            _view.ShowSettings(await Domain.RecordingSettings.LoadAsync());
        }
    }
}