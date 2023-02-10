using MicrophoneLevelLogger.Domain;

namespace MicrophoneLevelLogger.Command;

public class DeleteRecordCommand : ICommand
{
    public string Name => "Delete records       : Recodeで保存したデータをすべて削除する。";
    public Task ExecuteAsync()
    {
        if (Directory.Exists(RecordCommand.RecordDirectoryName))
        {
            Directory.Delete(RecordCommand.RecordDirectoryName, true);
        }
        return Task.CompletedTask;
    }
}