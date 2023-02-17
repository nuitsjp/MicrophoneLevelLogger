namespace MicrophoneLevelLogger.Client.Controller.Measure;

public class MeasureController : IController
{
    private readonly IMeasureView _view;
    private readonly IAudioInterfaceProvider _provider;
    private readonly IMediaPlayer _mediaPlayer;
    private readonly IAudioInterfaceInputLevelsRepository _repository;
    private readonly IAudioInterfaceLoggerProvider _loggerProvider;

    public MeasureController(
        IMeasureView view,
        IAudioInterfaceProvider provider,
        IMediaPlayer mediaPlayer, 
        IAudioInterfaceInputLevelsRepository repository, IAudioInterfaceLoggerProvider loggerProvider)
    {
        _view = view;
        _provider = provider;
        _mediaPlayer = mediaPlayer;
        _repository = repository;
        _loggerProvider = loggerProvider;
    }

    public string Name => "Measure              : 指定マイクの入力音量を計測する。";

    public async Task ExecuteAsync()
    {
        // マイクを選択し、有効化する
        var audioInterface = _provider.Resolve();
        var microphone = _view.SelectMicrophone(audioInterface);

        // 計測時間を入力する
        var span = TimeSpan.FromSeconds(_view.InputSpan());

        // メディアの再生可否を確認する
        var isPlayMedia = _view.ConfirmPlayMedia();

        if (isPlayMedia is false)
        {
            // 準備ができたか確認する
            while (_view.ConfirmReady() is false)
            {
            }
        }

        // 計測値の画面通知を開始する
        _view.StartNotifyMasterPeakValue(new AudioInterface(microphone));

        // 音声を再生する
        CancellationTokenSource source = new();
        if (isPlayMedia)
        {
            await _mediaPlayer.PlayLoopingAsync(source.Token);
        }

        // 計測を開始する
        var logger = _loggerProvider.ResolveLocal(microphone);
        await logger.StartAsync(source.Token);
        try
        {
            // 計測完了を待機する
            _view.Wait(span);

            // 計測結果リストを更新する
            AudioInterfaceInputLevels inputLevels = await _repository.LoadAsync();
            foreach (var microphoneLogger in logger.MicrophoneLoggers)
            {
                inputLevels.Update(
                    new MicrophoneInputLevel(
                        microphoneLogger.Microphone.Id,
                        microphoneLogger.Microphone.Name,
                        microphoneLogger.Min,
                        microphoneLogger.Avg,
                        microphoneLogger.Max
                    ));
            }
            await _repository.SaveAsync(inputLevels);

            // 結果を通知する
            _view.NotifyResult(logger);
        }
        finally
        {
            // リソースを停止・解放する。
            microphone.Deactivate();
            _view.StopNotifyMasterPeakValue();
            source.Cancel();
        }
    }
}