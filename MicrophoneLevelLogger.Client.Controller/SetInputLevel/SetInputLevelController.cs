namespace MicrophoneLevelLogger.Client.Controller.SetInputLevel;

public class SetInputLevelController : IController
{
    private readonly ISetInputLevelView _view;
    private readonly IAudioInterfaceProvider _audioInterfaceProvider;

    public SetInputLevelController(
        ISetInputLevelView view,
        IAudioInterfaceProvider audioInterfaceProvider)
    {
        _view = view;
        _audioInterfaceProvider = audioInterfaceProvider;
    }

    public string Name => "Set input level";
    public string Description => "指定マイクを入力レベルを変更する。";

    public async Task ExecuteAsync()
    {
        var audioInterface = _audioInterfaceProvider.Resolve();
        var microphone = _view.SelectMicrophone(audioInterface);
        var inputLevel = _view.InputInputLevel();
        microphone.VolumeLevel = new(inputLevel);

        await _view.NotifyAudioInterfaceAsync(audioInterface);
    }
}