namespace Quietrum;

/// <summary>
/// スピーカーID
/// </summary>
public interface ISpeaker
{
    /// <summary>
    /// ID
    /// </summary>
    SpeakerId Id { get; }
    /// <summary>
    /// 名称
    /// </summary>
    string Name { get; }
    /// <summary>
    /// 出力レベル
    /// </summary>
    VolumeLevel VolumeLevel { get; set; }
}