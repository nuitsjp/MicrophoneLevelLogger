namespace MicrophoneLevelLogger.Client.Controller;

/// <summary>
/// コントローラー
/// </summary>
public interface IController
{
    /// <summary>
    /// 名称
    /// </summary>
    string Name { get; }
    /// <summary>
    /// 概要
    /// </summary>
    string Description { get; }
    /// <summary>
    /// 実行する。
    /// </summary>
    /// <returns></returns>
    Task ExecuteAsync();
}