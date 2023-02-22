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

    public string Name => "Set Alias";
    public string Description => "マイクの別名を設定する。";

    public async Task ExecuteAsync()
    {
        var audioInterface = _provider.Resolve();

        var microphone = _view.SelectMicrophone(audioInterface);
        var name = _view.InputAlias(microphone);

        var settings = await _repository.LoadAsync();
        settings.UpdateAlias(
            new Alias(microphone.Id, microphone.SystemName, name));
        await _repository.SaveAsync(settings);
    }
}