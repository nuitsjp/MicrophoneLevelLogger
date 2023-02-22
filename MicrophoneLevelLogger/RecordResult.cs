namespace MicrophoneLevelLogger;

/// <summary>
/// 記録結果
/// </summary>
public class RecordResult
{
    /// <summary>
    /// インスタンスを生成する。
    /// </summary>
    /// <param name="no"></param>
    /// <param name="microphoneRecorder"></param>
    public RecordResult(int no, IMicrophoneRecorder microphoneRecorder)
    {
        No = no;
        Name = microphoneRecorder.Microphone.Name;
        Min = microphoneRecorder.Min;
        Avg = microphoneRecorder.Avg;
        Max = microphoneRecorder.Max;
    }

    /// <summary>
    /// インスタンスを生成する。
    /// </summary>
    /// <param name="no"></param>
    /// <param name="name"></param>
    /// <param name="min"></param>
    /// <param name="avg"></param>
    /// <param name="max"></param>
    public RecordResult(int no, string name, Decibel min, Decibel avg, Decibel max)
    {
        No = no;
        Name = name;
        Min = min;
        Avg = avg;
        Max = max;
    }

    /// <summary>
    /// マイクの表示番号
    /// </summary>
    public int No { get; }
    /// <summary>
    /// マイクの名称
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