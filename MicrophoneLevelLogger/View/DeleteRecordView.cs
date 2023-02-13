using MicrophoneLevelLogger.Command;
using MicrophoneLevelLogger.Command.DeleteRecord;
using Sharprompt;

namespace MicrophoneLevelLogger.View;

public class DeleteRecordView : IDeleteRecordView
{
    public bool Confirm()
    {
        return Prompt.Confirm("録音データをすべて削除しますか？", false);
    }
}