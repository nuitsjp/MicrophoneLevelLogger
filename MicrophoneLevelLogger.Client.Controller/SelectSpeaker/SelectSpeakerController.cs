namespace MicrophoneLevelLogger.Client.Controller.SelectSpeaker;

/// <summary>
/// 音源を再生するスピーカーを選択する。
/// </summary>
public class SelectSpeakerController : IController
{
    /// <summary>
    /// ビュー
    /// </summary>
    private readonly ISelectSpeakerView _view;
    /// <summary>
    /// IAudioInterfaceプロバイダー
    /// </summary>
    private readonly IAudioInterfaceProvider _provider;
    /// <summary>
    /// Settingsプロバイダー
    /// </summary>
    private readonly ISettingsRepository _repository;

    /// <summary>
    /// インスタンスを生成する。
    /// </summary>
    /// <param name="view"></param>
    /// <param name="provider"></param>
    /// <param name="repository"></param>
    public SelectSpeakerController(
        ISelectSpeakerView view, 
        IAudioInterfaceProvider provider, 
        ISettingsRepository repository)
    {
        _view = view;
        _provider = provider;
        _repository = repository;
    }
    /// <summary>
    /// 名称
    /// </summary>
    public string Name => "Select speaker";
    /// <summary>
    /// 概要
    /// </summary>
    public string Description => "音源を再生するスピーカーを選択する。";

    public async Task ExecuteAsync()
    {
        // スピーカーを選択する。
        var audioInterface = _provider.Resolve();
        if (_view.TrySelectSpeaker(
                audioInterface.GetSpeakers(), 
                await audioInterface.GetSpeakerAsync(),
                out var selected))
        {
            // 設定を更新する。
            var settings = await _repository.LoadAsync();
            await _repository.SaveAsync(
                new Settings(
                    settings.MediaPlayerHost,
                    settings.RecorderHost,
                    settings.RecordingSpan,
                    settings.IsEnableRemotePlaying,
                    settings.IsEnableRemoteRecording,
                    settings.Aliases,
                    settings.DisabledMicrophones,
                    selected.Id));
        }
    }
}