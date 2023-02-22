namespace MicrophoneLevelLogger.Client.Controller.SetMaxInputLevel;

public class SetMaxInputLevelController : IController
{
    private readonly IAudioInterfaceProvider _audioInterfaceProvider;

    public SetMaxInputLevelController(IAudioInterfaceProvider audioInterfaceProvider)
    {
        _audioInterfaceProvider = audioInterfaceProvider;
    }

    public string Name => "Set Max";
    public string Description => "マイクを入力レベルを最大に変更する。";

    public Task ExecuteAsync()
    {
        var audioInterface = _audioInterfaceProvider.Resolve();
        foreach (var microphone in audioInterface.GetMicrophones())
        {
            microphone.VolumeLevel = VolumeLevel.Maximum;
        }

        return Task.CompletedTask;
    }
}