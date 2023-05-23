namespace Quietrum;

/// <summary>
/// IAudioInterfaceのプロバイダー
/// </summary>
public interface IAudioInterfaceProvider
{
    /// <summary>
    /// IAudioInterfaceを解決する。
    /// </summary>
    /// <returns></returns>
    IAudioInterface Resolve();
}