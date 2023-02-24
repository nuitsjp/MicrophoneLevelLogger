namespace MicrophoneLevelLogger.Client.Controller.SetAlias;

/// <summary>
/// マイクの別名を設定する。
/// </summary>
public class SetAliasController : IController
{
    /// <summary>
    /// ビュー
    /// </summary>
    private readonly ISetAliasView _view;
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
    public SetAliasController(
        ISetAliasView view,
        IAudioInterfaceProvider provider, 
        ISettingsRepository repository)
    {
        _provider = provider;
        _repository = repository;
        _view = view;
    }
    /// <summary>
    /// 別名
    /// </summary>
    public string Name => "Set Alias";
    /// <summary>
    /// 概要
    /// </summary>
    public string Description => "マイクの別名を設定する。";

    public async Task ExecuteAsync()
    {
        var audioInterface = _provider.Resolve();
        // マイクを選択し、別名を入力する。
        var microphone = _view.SelectMicrophone(audioInterface);
        var name = _view.InputAlias(microphone);

        // 設定を更新する。
        var settings = await _repository.LoadAsync();
        settings.UpdateAlias(
            new Alias(microphone.Id, microphone.SystemName, name));
        await _repository.SaveAsync(settings);
    }
}