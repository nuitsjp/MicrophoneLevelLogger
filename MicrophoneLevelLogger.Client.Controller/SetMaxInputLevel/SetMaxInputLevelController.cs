namespace MicrophoneLevelLogger.Client.Controller.SetMaxInputLevel;

public class SetMaxInputLevelController : IController
{
    private readonly IAudioInterfaceProvider _audioInterfaceProvider;

    public SetMaxInputLevelController(IAudioInterfaceProvider audioInterfaceProvider)
    {
        _audioInterfaceProvider = audioInterfaceProvider;
    }

    public string Name => "Set Maximum          : マイクを入力レベルを最大に変更する。";

    public async Task ExecuteAsync()
    {
        var audioInterface = await _audioInterfaceProvider.ResolveAsync();
        foreach (var microphone in audioInterface.Microphones)
        {
            microphone.VolumeLevel = VolumeLevel.Maximum;
        }
    }
}