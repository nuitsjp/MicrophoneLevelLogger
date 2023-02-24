namespace MicrophoneLevelLogger.Client.Controller.MonitorVolume;

/// <summary>
/// マイクの入力音量をモニターする。
/// </summary>
public class MonitorVolumeController : IController
{
    /// <summary>
    /// ビュー
    /// </summary>
    private readonly IMonitorVolumeView _view;
    /// <summary>
    /// IAudioInterfaceプロバイダー
    /// </summary>
    private readonly IAudioInterfaceProvider _audioInterfaceProvider;
    /// <summary>
    /// IRecorderプロバイダー
    /// </summary>
    private readonly IRecorderProvider _recorderProvider;

    /// <summary>
    /// インスタンスを生成する。
    /// </summary>
    /// <param name="audioInterfaceProvider"></param>
    /// <param name="view"></param>
    /// <param name="recorderProvider"></param>
    public MonitorVolumeController(
        IAudioInterfaceProvider audioInterfaceProvider, 
        IMonitorVolumeView view, 
        IRecorderProvider recorderProvider)
    {
        _audioInterfaceProvider = audioInterfaceProvider;
        _recorderProvider = recorderProvider;
        _view = view;
    }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name => "Monitor volume";
    /// <summary>
    /// 概要
    /// </summary>
    public string Description => "マイクの入力をモニターする。データの保存は行わない。";

    public async Task ExecuteAsync()
    {
        // オーディオインターフェースからレコーダーを解決する。
        var audioInterface = _audioInterfaceProvider.Resolve();
         var recorder = _recorderProvider.ResolveLocal(audioInterface, null);

        // モニターを開始する
        CancellationTokenSource source = new();
        await recorder.StartAsync(source.Token);
        try
        {
            // 画面に入力を通知する
            _view.StartNotify(recorder, source.Token);

            // モニター終了を待機する。
            _view.WaitToBeStopped();
        }
        finally
        {
            // モニターを中断する。
            source.Cancel();
        }
    }
}