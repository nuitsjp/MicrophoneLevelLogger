using FluentTextTable;
using MicrophoneLevelLogger.Command;
using MicrophoneLevelLogger.Domain;

namespace MicrophoneLevelLogger.View;

public class RecordView : MicrophoneView, IRecordView
{
    public void NotifyResult(IEnumerable<RecordResult> results)
    {
        Build
            .TextTable<RecordResult>(builder =>
            {
                builder.Borders.InsideHorizontal.AsDisable();
            })
            .WriteLine(results);
    }
}