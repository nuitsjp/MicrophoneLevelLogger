﻿using MicrophoneLevelLogger.Client.Controller.Record;

namespace MicrophoneLevelLogger.Client.Controller.DeleteRecord;

public class DeleteRecordController : IController
{
    private readonly IDeleteRecordView _view;

    public DeleteRecordController(IDeleteRecordView view)
    {
        _view = view;
    }

    public string Name => "Delete records       : Recodeで保存したデータをすべて削除する。";
    public Task ExecuteAsync()
    {
        if (_view.Confirm())
        {
            if (Directory.Exists(RecordController.RecordDirectoryName))
            {
                Directory.Delete(RecordController.RecordDirectoryName, true);
            }
        }
        return Task.CompletedTask;
    }
}