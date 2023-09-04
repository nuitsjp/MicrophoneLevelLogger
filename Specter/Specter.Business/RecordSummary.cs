using System;
using System.Collections.Generic;
using System.Linq;

namespace Specter.Business;

/// <summary>
/// 記録サマリー
/// </summary>
public class RecordSummary
{
    /// <summary>
    /// マイク別記録サマリー
    /// </summary>
    private readonly List<MicrophoneRecordSummary> _microphones;

    /// <summary>
    /// インスタンスを生成する。
    /// </summary>
    /// <param name="name"></param>
    /// <param name="begin"></param>
    /// <param name="end"></param>
    /// <param name="microphones"></param>
    public RecordSummary(
        string name,
        DateTime begin, 
        DateTime end,
        IReadOnlyList<MicrophoneRecordSummary> microphones)
    {
        Name = name;
        Begin = begin;
        End = end;
        _microphones = microphones.ToList();
    }

    /// <summary>
    /// マイク名称
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// 記録開始時刻
    /// </summary>
    public DateTime Begin { get; }
    /// <summary>
    /// 記録終了時刻
    /// </summary>
    public DateTime End { get; }
    /// <summary>
    /// マイク別記録サマリーを取得する。
    /// </summary>
    public IReadOnlyList<MicrophoneRecordSummary> Microphones => _microphones;
}