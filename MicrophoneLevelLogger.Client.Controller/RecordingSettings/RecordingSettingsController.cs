﻿namespace MicrophoneLevelLogger.Client.Controller.RecordingSettings;

public class RecordingSettingsController : IController
{
    private readonly IRecordingSettingsView _view;
    private readonly IRecordingSettingsRepository _repository;

    public RecordingSettingsController(
        IRecordingSettingsView view, 
        IRecordingSettingsRepository repository)
    {
        _view = view;
        _repository = repository;
    }

    public string Name => "Recoding Settings    : 録音設定を確認・変更する。";

    public async Task ExecuteAsync()
    {
        var settings = await _repository.LoadAsync();
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

            await _repository.SaveAsync(
                new MicrophoneLevelLogger.RecordingSettings(
                    mediaPlayerHost,
                    recorderHost,
                    TimeSpan.FromSeconds(recordingSpan),
                    isEnableRemotePlaying,
                    isEnableRemoteRecording));

            _view.ShowSettings(await _repository.LoadAsync());
        }
    }
}