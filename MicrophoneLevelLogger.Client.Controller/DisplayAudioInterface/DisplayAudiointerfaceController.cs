namespace MicrophoneLevelLogger.Client.Controller.DisplayAudioInterface;

/// <summary>
/// オーディオインターフェースの状態を表示する。
/// </summary>
public class DisplayAudioInterfaceController : IController
{
    /// <summary>
    /// ビュー
    /// </summary>
    private readonly IMicrophoneView _view;
    /// <summary>
    /// IAudioInterfaceProviderプロバイダー
    /// </summary>
    private readonly IAudioInterfaceProvider _provider;

    /// <summary>
    /// インスタンスを生成する。
    /// </summary>
    /// <param name="view"></param>
    /// <param name="provider"></param>
    public DisplayAudioInterfaceController(
        IMicrophoneView view, 
        IAudioInterfaceProvider provider)
    {
        _view = view;
        _provider = provider;
    }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name => "Display interface";
    /// <summary>
    /// 概要
    /// </summary>
    public string Description => "マイクやスピーカーの情報を表示します。";

    public async Task ExecuteAsync()
    {
        await _view.NotifyAudioInterfaceAsync(_provider.Resolve());
    }
}