namespace MicrophoneLevelLogger;

/// <summary>
/// マイク状態
/// </summary>
[Flags]
public enum MicrophoneStatus
{
    /// <summary>
    /// 無効
    /// </summary>
    Disable = 0x01,
    /// <summary>
    /// 有効
    /// </summary>
    Enable = 0x02
}