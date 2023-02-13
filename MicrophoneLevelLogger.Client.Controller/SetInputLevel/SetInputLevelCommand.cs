namespace MicrophoneLevelLogger.Client.Controller.SetInputLevel;

public class SetInputLevelCommand : ICommand
{
    private readonly ISetInputLevelView _view;
    private readonly IAudioInterfaceProvider _audioInterfaceProvider;

    public SetInputLevelCommand(
        ISetInputLevelView view,
        IAudioInterfaceProvider audioInterfaceProvider)
    {
        _view = view;
        _audioInterfaceProvider = audioInterfaceProvider;
    }

    public string Name => "Set input level      : 指定マイクを入力レベルを変更する。";
    public Task ExecuteAsync()
    {
        var audioInterface = _audioInterfaceProvider.Resolve();
        var microphone = _view.SelectMicrophone(audioInterface);
        var inputLevel = _view.InputInputLevel();
        microphone.VolumeLevel = new(inputLevel);

        _view.NotifyMicrophonesInformation(audioInterface);

        return Task.CompletedTask; ;
    }
}