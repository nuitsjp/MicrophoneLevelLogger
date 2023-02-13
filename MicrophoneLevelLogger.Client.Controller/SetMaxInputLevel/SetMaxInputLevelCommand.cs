namespace MicrophoneLevelLogger.Client.Controller.SetMaxInputLevel;

public class SetMaxInputLevelCommand : ICommand
{
    private readonly IAudioInterfaceProvider _audioInterfaceProvider;

    public SetMaxInputLevelCommand(IAudioInterfaceProvider audioInterfaceProvider)
    {
        _audioInterfaceProvider = audioInterfaceProvider;
    }

    public string Name => "Set Maximum          : マイクを入力レベルを最大に変更する。";

    public Task ExecuteAsync()
    {
        foreach (var microphone in _audioInterfaceProvider.Resolve().Microphones)
        {
            microphone.VolumeLevel = VolumeLevel.Maximum;
        }

        return Task.CompletedTask;
    }
}