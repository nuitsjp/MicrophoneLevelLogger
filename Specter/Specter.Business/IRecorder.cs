namespace Specter.Business;

/// <summary>
/// AudioInterfaceのレコーダー
/// </summary>
public interface IRecorder
{
    /// <summary>
    /// マイク別のレコーダー
    /// </summary>
    IReadOnlyList<IMicrophoneRecorder> MicrophoneRecorders { get; }
    /// <summary>
    /// 録音を開始する。
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task StartAsync(CancellationToken token);
    /// <summary>
    /// 該当マイクのレコーダーを取得する。
    /// </summary>
    /// <param name="device"></param>
    /// <returns></returns>
    public IMicrophoneRecorder GetLogger(IDevice device);
}