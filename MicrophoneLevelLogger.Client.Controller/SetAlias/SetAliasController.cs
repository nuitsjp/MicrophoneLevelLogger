namespace MicrophoneLevelLogger.Client.Controller.SetAlias;

public class SetAliasController : IController
{
    private readonly ISetAliasView _view;
    private readonly IAudioInterfaceProvider _provider;
    private readonly ISettingsRepository _repository;

    public SetAliasController(
        ISetAliasView view,
        IAudioInterfaceProvider provider, 
        ISettingsRepository repository)
    {
        _provider = provider;
        _repository = repository;
        _view = view;
    }

    public string Name => "Set Alias            : マイクの別名を設定する。";

    public async Task ExecuteAsync()
    {
        var audioInterface = await _provider.ResolveAsync();

        var microphone = _view.SelectMicrophone(audioInterface);
        var name = _view.InputAlias(microphone);

        var settings = await _repository.LoadAsync();
        settings.UpdateAlias(
            new Alias(microphone.Id, microphone.SystemName, name));
        await _repository.SaveAsync(settings);
    }
}