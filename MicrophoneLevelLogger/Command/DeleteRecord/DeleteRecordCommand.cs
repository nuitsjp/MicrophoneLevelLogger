﻿using MicrophoneLevelLogger.Command.Record;

namespace MicrophoneLevelLogger.Command.DeleteRecord;

public class DeleteRecordCommand : ICommand
{
    private readonly IDeleteRecordView _view;

    public DeleteRecordCommand(IDeleteRecordView view)
    {
        _view = view;
    }

    public string Name => "Delete records       : Recodeで保存したデータをすべて削除する。";
    public Task ExecuteAsync()
    {
        if (_view.Confirm())
        {
            if (Directory.Exists(RecordCommand.RecordDirectoryName))
            {
                Directory.Delete(RecordCommand.RecordDirectoryName, true);
            }
        }
        return Task.CompletedTask;
    }
}