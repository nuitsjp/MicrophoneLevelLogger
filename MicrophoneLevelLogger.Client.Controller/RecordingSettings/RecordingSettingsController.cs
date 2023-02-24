namespace MicrophoneLevelLogger.Client.Controller.RecordingSettings;

/// <summary>
/// 記録設定を確認・変更する。
/// </summary>
public class RecordingSettingsController : IController
{
    /// <summary>
    /// ビュー
    /// </summary>
    private readonly IRecordingSettingsView _view;
    /// <summary>
    /// Settingリポジトリー
    /// </summary>
    private readonly ISettingsRepository _repository;
    /// <summary>
    /// インスタンスを生成する。
    /// </summary>
    /// <param name="view"></param>
    /// <param name="repository"></param>
    public RecordingSettingsController(
        IRecordingSettingsView view, 
        ISettingsRepository repository)
    {
        _view = view;
        _repository = repository;
    }
    /// <summary>
    /// 名称
    /// </summary>
    public string Name => "Recoding Settings";
    /// <summary>
    /// 概要
    /// </summary>
    public string Description => "記録設定を確認・変更する。";

    public async Task ExecuteAsync()
    {
        // 現在の設定を表示する。
        var settings = await _repository.LoadAsync();
        _view.ShowSettings(settings);

        // 変更するかどうか、確認する。
        if (_view.ConfirmModify())
        {
            // 記録時間を入力する。
            var recordingSpan = _view.InputRecodingSpan();

            // リモート記録を設定する。
            var isEnableRemoteRecording = _view.ConfirmEnableRemoteRecording();
            var recorderHost = isEnableRemoteRecording
                ? _view.InputRecorderHost()
                : "localhost";

            // リモート再生を設定する。
            var isEnableRemotePlaying = _view.ConfirmEnableRemotePlaying();
            var mediaPlayerHost = isEnableRemotePlaying
                ? _view.InputMediaPlayerHost()
                : "localhost";

            // 設定を保存する。
            await _repository.SaveAsync(
                new Settings(
                    mediaPlayerHost,
                    recorderHost,
                    TimeSpan.FromSeconds(recordingSpan),
                    isEnableRemotePlaying,
                    isEnableRemoteRecording,
                    settings.Aliases,
                    settings.DisabledMicrophones,
                    settings.SelectedSpeakerId));

            // 設定結果を表示する。
            _view.ShowSettings(await _repository.LoadAsync());
        }
    }
}