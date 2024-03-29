﻿using FluentTextTable;
using MicrophoneLevelLogger.Client.Controller.DisplayRecords;
using Sharprompt;
// ReSharper disable LocalizableElement

namespace MicrophoneLevelLogger.Client.View;

/// <summary>
/// 記録表示ビュー
/// </summary>
public class DisplayRecordsView : IDisplayRecordsView
{
    /// <summary>
    /// 表示内容を選択する。
    /// </summary>
    /// <returns></returns>
    public DisplayType SelectDisplayRecordsType()
    {
        var types = new[]
        {
            "マイク別の最新の記録",
            "記録別のサマリー"
        };
        var type = Prompt.Select("", types);
        return type == types[0]
            ? DisplayType.MicrophoneView
            : DisplayType.RecordView;
    }
    /// <summary>
    /// 表示する記録を選択する。
    /// </summary>
    /// <param name="summaries"></param>
    /// <returns></returns>
    public RecordSummary SelectRecordSummary(IEnumerable<RecordSummary> summaries)
    {
        var records = summaries.ToList();
        var items = records
            .OrderBy(x => x.Begin)
            .Select(x => $"{x.Begin:yyyy/MM/dd HH:mm:ss} - {x.Name}").ToList();
        var summary = Prompt.Select("", items);
        var index = items.IndexOf(summary);
        return records[index];
    }
    /// <summary>
    /// 記録を表示する。
    /// </summary>
    /// <param name="summary"></param>
    public void Display(RecordSummary summary)
    {
        lock (this)
        {
            Console.WriteLine();
            Console.WriteLine($"name: {summary.Name}");
            Console.WriteLine($"開始: {summary.Begin:yyyy/MM/dd HH:mm:ss}");
            Console.WriteLine($"終了: {summary.End:yyyy/MM/dd HH:mm:ss}");
            Build
                .TextTable<RecordResult>(builder =>
                {
                    builder.Borders.InsideHorizontal.AsDisable();
                    builder.Columns.Add(x => x.No);
                    builder.Columns.Add(x => x.Name);
                    builder.Columns.Add(x => x.Min).FormatAs("{0:#.00}");
                    builder.Columns.Add(x => x.Avg).FormatAs("{0:#.00}");
                    builder.Columns.Add(x => x.Max).FormatAs("{0:#.00}");
                })
                .WriteLine(
                    summary.Microphones
                        .Select((x, index) => 
                            new RecordResult(
                                index + 1, 
                                x.Name,
                                x.Min,
                                x.Avg,
                                x.Max)));
            Console.WriteLine();
        }
    }
    /// <summary>
    /// マイク別の記録を表示する。
    /// </summary>
    /// <param name="summaries"></param>
    public void Display(IEnumerable<MicrophoneRecordSummary> summaries)
    {
        lock (this)
        {
            Console.WriteLine();
            Build
                .TextTable<RecordResult>(builder =>
                {
                    builder.Borders.InsideHorizontal.AsDisable();
                    builder.Columns.Add(x => x.No);
                    builder.Columns.Add(x => x.Name);
                    builder.Columns.Add(x => x.Min).FormatAs("{0:#.00}");
                    builder.Columns.Add(x => x.Avg).FormatAs("{0:#.00}");
                    builder.Columns.Add(x => x.Max).FormatAs("{0:#.00}");
                })
                .WriteLine(
                    summaries
                        .Select((x, index) =>
                            new RecordResult(
                                index + 1,
                                x.Name,
                                x.Min,
                                x.Avg,
                                x.Max)));
            Console.WriteLine();
        }
    }
}