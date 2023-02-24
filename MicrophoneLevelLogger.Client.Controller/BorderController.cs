namespace MicrophoneLevelLogger.Client.Controller;

/// <summary>
/// コントローラーに罫線を表示するためのダミーコントローラー
/// </summary>
public class BorderController : IController
{
    public string Name => "-----------------------------------------------------------";
    public string Description => "-----------------------------------------------------------";
    public Task ExecuteAsync() => Task.CompletedTask;
}