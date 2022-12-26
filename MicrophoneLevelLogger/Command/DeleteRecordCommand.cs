using MicrophoneLevelLogger.Domain;

namespace MicrophoneLevelLogger.Command;

public class DeleteRecordCommand : ICommand
{
    private readonly IMicrophonesProvider _provider;

    public DeleteRecordCommand(IMicrophonesProvider provider)
    {
        _provider = provider;
    }

    public string Name => "Delete record files";
    public Task ExecuteAsync()
    {
        using var microphones = _provider.Resolve();
        microphones.DeleteRecordFiles();
        return Task.CompletedTask;
    }
}