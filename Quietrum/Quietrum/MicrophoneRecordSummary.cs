namespace Quietrum;

/// <summary>
/// マイク録音サマリー
/// </summary>
public class MicrophoneRecordSummary
{
    /// <summary>
    /// インスタンスを生成する。
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="min"></param>
    /// <param name="avg"></param>
    /// <param name="max"></param>
    public MicrophoneRecordSummary(
        DeviceId id, 
        string name, 
        Decibel min, 
        Decibel avg, 
        Decibel max)
    {
        Id = id;
        Name = name;
        Min = min;
        Avg = avg;
        Max = max;
    }

    /// <summary>
    /// マイクID
    /// </summary>
    public DeviceId Id { get; }
    /// <summary>
    /// マイク名称
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// 最小音量
    /// </summary>
    public Decibel Min { get; }
    /// <summary>
    /// 平均音量
    /// </summary>
    public Decibel Avg { get; }
    /// <summary>
    /// 最大音量
    /// </summary>
    public Decibel Max { get; }

}