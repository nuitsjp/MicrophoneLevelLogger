using MicrophoneLevelLogger.Domain;

namespace MicrophoneLevelLogger.Command;

public class SetMaxInputLevelCommand : ICommand
{
    private readonly IMicrophonesProvider _microphonesProvider;

    public SetMaxInputLevelCommand(IMicrophonesProvider microphonesProvider)
    {
        _microphonesProvider = microphonesProvider;
    }

    public string Name => "Set microphone input level to maximum";

    public Task ExecuteAsync()
    {
        foreach (var microphone in _microphonesProvider.Resolve().Devices)
        {
            microphone.MasterVolumeLevelScalar = MasterVolumeLevelScalar.Maximum;
        }

        return Task.CompletedTask;
    }
}