using MicrophoneLevelLogger.Domain;

namespace MicrophoneLevelLogger.Command;

public class SetMaxInputLevelCommand : ICommand
{
    private readonly IMicrophonesProvider _microphonesProvider;

    public SetMaxInputLevelCommand(IMicrophonesProvider microphonesProvider)
    {
        _microphonesProvider = microphonesProvider;
    }

    public string Name => "Set Maximum : マイクを入力レベルを最大に変更する。";

    public Task ExecuteAsync()
    {
        foreach (var microphone in _microphonesProvider.Resolve().Devices)
        {
            microphone.MasterVolumeLevelScalar = MasterVolumeLevelScalar.Maximum;
        }

        return Task.CompletedTask;
    }
}