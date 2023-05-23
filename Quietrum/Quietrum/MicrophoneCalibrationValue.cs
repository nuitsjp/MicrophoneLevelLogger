namespace Quietrum;

/// <summary>
/// マイクのキャリブレーション結果
/// </summary>
public class MicrophoneCalibrationValue
{
    /// <summary>
    /// インスタンスを生成する。
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="volumeLevel"></param>
    public MicrophoneCalibrationValue(
        MicrophoneId id,
        string name,
        VolumeLevel volumeLevel)
    {
        Id = id;
        Name = name;
        VolumeLevel = volumeLevel;
    }

    /// <summary>
    /// ID
    /// </summary>
    public MicrophoneId Id { get; }
    /// <summary>
    /// 名前
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// 入力レベル
    /// </summary>
    public VolumeLevel VolumeLevel { get; }
}