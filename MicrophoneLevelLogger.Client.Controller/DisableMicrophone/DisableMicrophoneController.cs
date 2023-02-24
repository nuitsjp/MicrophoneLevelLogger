namespace MicrophoneLevelLogger.Client.Controller.DisableMicrophone;

/// <summary>
/// 指定のマイクを無効化する。
/// </summary>
public class DisableMicrophoneController : IController
{
    /// <summary>
    /// ビュー
    /// </summary>
    private readonly IDisableMicrophoneView _view;
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
    public DisableMicrophoneController(
        IDisableMicrophoneView view, 
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
    public string Name => "Disable microphone";
    /// <summary>
    /// 概要
    /// </summary>
    public string Description => "任意のマイクを無効化する。";

    public async Task ExecuteAsync()
    {
        // 無効化するマイクを選択する。
        var audioInterface = _provider.Resolve();
        if (_view.TrySelectMicrophone(audioInterface, out var microphone))
        {
            // 無効化するマイクを設定に保存する。
            var settings = await _repository.LoadAsync();
            settings.DisableMicrophone(microphone.Id);
            await _repository.SaveAsync(settings);

            // 有効化されているマイクを表示する。
            await _view.NotifyAudioInterfaceAsync(_provider.Resolve());
        }

    }
}