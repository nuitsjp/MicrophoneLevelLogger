using FluentTextTable;
using MicrophoneLevelLogger.Command;
using MicrophoneLevelLogger.Domain;

namespace MicrophoneLevelLogger.View;

public class RecordView : MicrophoneView, IRecordView
{
    public void NotifyResult(IEnumerable<RecordResult> results)
    {
        lock (this)
        {
            Build
                .TextTable<RecordResult>(builder =>
                {
                    builder.Borders.InsideHorizontal.AsDisable();
                    builder.Columns.Add(x => x.No);
                    builder.Columns.Add(x => x.Name);
                    builder.Columns.Add(x => x.Min).FormatAs("{0:#.00}");
                    builder.Columns.Add(x => x.Avg).FormatAs("{0:#.00}");
                    builder.Columns.Add(x => x.Median).FormatAs("{0:#.00}");
                    builder.Columns.Add(x => x.Max).FormatAs("{0:#.00}");
                })
                .WriteLine(results);
        }
    }
}