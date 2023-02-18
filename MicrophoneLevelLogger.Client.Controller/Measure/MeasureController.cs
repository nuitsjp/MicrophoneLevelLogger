namespace MicrophoneLevelLogger.Client.Controller.Measure;

public class MeasureController : IController
{
    private readonly IMeasureView _view;
    private readonly IAudioInterfaceProvider _audioInterfaceProvider;
    private readonly IMediaPlayer _mediaPlayer;
    private readonly IAudioInterfaceInputLevelsRepository _repository;
    private readonly IRecorderProvider _loggerProvider;

    public MeasureController(
        IMeasureView view,
        IAudioInterfaceProvider audioInterfaceProvider,
        IMediaPlayer mediaPlayer, 
        IAudioInterfaceInputLevelsRepository repository, IRecorderProvider loggerProvider)
    {
        _view = view;
        _audioInterfaceProvider = audioInterfaceProvider;
        _mediaPlayer = mediaPlayer;
        _repository = repository;
        _loggerProvider = loggerProvider;
    }

    public string Name => "Measure              : 指定マイクの入力音量を計測する。";

    public async Task ExecuteAsync()
    {
        // マイクを選択する
        var audioInterface = _audioInterfaceProvider.Resolve();
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
        CancellationTokenSource source = new();
        try
        {
            var recorder = _loggerProvider.ResolveLocal(microphone);

            // 画面への通知を開始する
            _view.StartNotify(recorder, source.Token);

            // 音声を再生する
            if (isPlayMedia)
            {
                await _mediaPlayer.PlayLoopingAsync(source.Token);
            }

            // 計測を開始する
            await recorder.StartAsync(source.Token);
            // 計測完了を待機する
            _view.Wait(span);

            // 計測結果リストを更新する
            AudioInterfaceInputLevels inputLevels = await _repository.LoadAsync();
            recorder.MicrophoneRecorders
                .ForEach(microphoneLogger =>
                {
                    inputLevels.Update(
                        new MicrophoneInputLevel(
                            microphoneLogger.Microphone.Id,
                            microphoneLogger.Microphone.Name,
                            microphoneLogger.Min,
                            microphoneLogger.Avg,
                            microphoneLogger.Max
                        ));
                });
            await _repository.SaveAsync(inputLevels);

            // 結果を通知する
            _view.NotifyResult(recorder);
        }
        finally
        {
            // リソースを停止・解放する。
            source.Cancel();
        }
    }
}