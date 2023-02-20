namespace MicrophoneLevelLogger.Client.Controller.RemoveAlias;

public class RemoveAliasController : IController
{
    private readonly IRemoveAliasView _view;
    private readonly ISettingsRepository _repository;

    public RemoveAliasController(
        IRemoveAliasView view, 
        ISettingsRepository repository)
    {
        _view = view;
        _repository = repository;
    }

    public string Name => "Remove Alias         : マイクの別名を削除する。";
    public async Task ExecuteAsync()
    {
        var settings = await _repository.LoadAsync();

        // 削除対象のエイリアスを選択し削除する。
        if (_view.TrySelectRemoveAlias(settings, out var alias))
        {
            settings.RemoveAlias(alias);
            await _repository.SaveAsync(settings);
        }
    }
}