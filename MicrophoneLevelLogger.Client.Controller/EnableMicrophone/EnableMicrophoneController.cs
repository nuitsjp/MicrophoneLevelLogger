namespace MicrophoneLevelLogger.Client.Controller.EnableMicrophone;

/// <summary>
/// 無効化済みのマイクを有効化する。
/// </summary>
public class EnableMicrophoneController : IController
{
    /// <summary>
    /// ビュー
    /// </summary>
    private readonly IEnableMicrophoneView _view;
    /// <summary>
    /// IAudioInterfaceプロバイダー
    /// </summary>
    private readonly IAudioInterfaceProvider _provider;
    /// <summary>
    /// Settingsリポジトリー
    /// </summary>
    private readonly ISettingsRepository _repository;

    /// <summary>
    /// インスタンスを生成する。
    /// </summary>
    /// <param name="view"></param>
    /// <param name="provider"></param>
    /// <param name="repository"></param>
    public EnableMicrophoneController(
        IEnableMicrophoneView view, 
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
    public string Name => "Enable microphone";
    /// <summary>
    /// 概要
    /// </summary>
    public string Description => "任意のマイクを有効化する。";

    public async Task ExecuteAsync()
    {
        // 無効化されているマイクを選択する。
        var audioInterface = _provider.Resolve();
        var settings = await _repository.LoadAsync();
        if (_view.TrySelectMicrophone(audioInterface, settings, out var microphone))
        {
            // 選択されたマイクを有効化する。
            settings.EnableMicrophone(microphone.Id);
            await _repository.SaveAsync(settings);

            // オーディオインターフェースの状態を通知する。
            await _view.NotifyAudioInterfaceAsync(_provider.Resolve());
        }
    }
}