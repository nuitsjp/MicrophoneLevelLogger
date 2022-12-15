using FluentTextTable;
using MicrophoneLevelLogger.Domain;

namespace MicrophoneLevelLogger.View;

public class RecordView : MicrophoneView, IRecordView
{
    public void NotifyResult(IEnumerable<IMasterPeakValues> values)
    {
        var results = values
            .Select((x, index) => new Result(index + 1, x))
            .ToList();
        Build
            .TextTable<Result>(builder =>
            {
                builder.Borders.InsideHorizontal.AsDisable();
            })
            .WriteLine(results);

    }

    public class Result
    {
        public Result(int no, IMasterPeakValues masterPeakValues)
        {
            No = no;
            Name = masterPeakValues.Microphone.Name;
            Min = masterPeakValues.PeakValues.Min();
            Avg = masterPeakValues.PeakValues.Average();
            Max = masterPeakValues.PeakValues.Max();
        }
        public int No { get; }
        public string Name { get; }
        public float Min { get; }
        public float Avg { get; }
        public float Max { get; }
    }

}