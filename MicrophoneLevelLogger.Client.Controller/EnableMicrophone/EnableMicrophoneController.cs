namespace MicrophoneLevelLogger.Client.Controller.EnableMicrophone;

public class EnableMicrophoneController : IController
{
    private readonly IEnableMicrophoneView _view;
    private readonly IAudioInterfaceProvider _provider;
    private readonly ISettingsRepository _repository;

    public EnableMicrophoneController(
        IEnableMicrophoneView view, 
        IAudioInterfaceProvider provider, 
        ISettingsRepository repository)
    {
        _view = view;
        _provider = provider;
        _repository = repository;
    }

    public string Name => "Enable microphone    : 任意のマイクを有効化する。";
    public async Task ExecuteAsync()
    {
        var audioInterface = _provider.Resolve();
        var settings = await _repository.LoadAsync();
        if (_view.TrySelectMicrophone(audioInterface, settings, out var microphone))
        {
            settings.EnableMicrophone(microphone.Id);
            await _repository.SaveAsync(settings);

            _view.NotifyMicrophonesInformation(_provider.Resolve());
        }
    }
}