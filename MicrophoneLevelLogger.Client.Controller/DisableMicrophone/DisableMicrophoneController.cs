namespace MicrophoneLevelLogger.Client.Controller.DisableMicrophone;

public class DisableMicrophoneController : IController
{
    private readonly IDisableMicrophoneView _view;
    private readonly IAudioInterfaceProvider _provider;
    private readonly ISettingsRepository _repository;

    public DisableMicrophoneController(
        IDisableMicrophoneView view, 
        IAudioInterfaceProvider provider, 
        ISettingsRepository repository)
    {
        _view = view;
        _provider = provider;
        _repository = repository;
    }

    public string Name => "Disable microphone   : 任意のマイクを無効化する。";

    public async Task ExecuteAsync()
    {
        var audioInterface = _provider.Resolve();
        if (_view.TrySelectMicrophone(audioInterface, out var microphone))
        {
            var settings = await _repository.LoadAsync();
            settings.DisableMicrophone(microphone.Id);
            await _repository.SaveAsync(settings);

            _view.NotifyMicrophonesInformation(_provider.Resolve());
        }

    }
}