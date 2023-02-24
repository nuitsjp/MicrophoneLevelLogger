namespace MicrophoneLevelLogger.Client.Controller.RemoveAlias;

/// <summary>
/// マイクの別名を削除する。
/// </summary>
public class RemoveAliasController : IController
{
    /// <summary>
    /// ビュー
    /// </summary>
    private readonly IRemoveAliasView _view;
    /// <summary>
    /// Settingリポジトリー
    /// </summary>
    private readonly ISettingsRepository _repository;

    /// <summary>
    /// インスタンスを生成する。
    /// </summary>
    /// <param name="view"></param>
    /// <param name="repository"></param>
    public RemoveAliasController(
        IRemoveAliasView view, 
        ISettingsRepository repository)
    {
        _view = view;
        _repository = repository;
    }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name => "Remove Alias";
    /// <summary>
    /// 概要
    /// </summary>
    public string Description => "マイクの別名を削除する。";

    public async Task ExecuteAsync()
    {
        // 設定をロードする。
        var settings = await _repository.LoadAsync();

        // 削除対象のエイリアスを選択する。
        if (_view.TrySelectRemoveAlias(settings, out var alias))
        {
            // 選択されたエイリアスを削除する。
            settings.RemoveAlias(alias);
            await _repository.SaveAsync(settings);
        }
    }
}