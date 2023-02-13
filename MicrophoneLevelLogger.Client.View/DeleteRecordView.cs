using MicrophoneLevelLogger.Client.Command.DeleteRecord;
using Sharprompt;

namespace MicrophoneLevelLogger.Client.View;

public class DeleteRecordView : IDeleteRecordView
{
    public bool Confirm()
    {
        return Prompt.Confirm("録音データをすべて削除しますか？", false);
    }
}