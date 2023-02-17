namespace MicrophoneLevelLogger.Client.Controller.CalibrateOutput;

public class CalibrateOutputController : IController
{
    private readonly IAudioInterfaceProvider _audioInterfaceProvider;
    private readonly ICalibrateOutputView _view;
    private readonly IMediaPlayerProvider _mediaPlayerProvider;
    private readonly IRecordingSettingsRepository _recordingSettingsRepository;

    public CalibrateOutputController(
        IAudioInterfaceProvider audioInterfaceProvider, 
        ICalibrateOutputView view, 
        IMediaPlayerProvider mediaPlayerProvider, 
        IRecordingSettingsRepository recordingSettingsRepository)
    {
        _audioInterfaceProvider = audioInterfaceProvider;
        _view = view;
        _mediaPlayerProvider = mediaPlayerProvider;
        _recordingSettingsRepository = recordingSettingsRepository;
    }

    public string Name => "Calibrate output     : スピーカーの出力レベルを調整する。";
    public async Task ExecuteAsync()
    {
        // マイクを選択し、有効化する
        var audioInterface = _audioInterfaceProvider.Resolve();
        var microphone = _view.SelectMicrophone(audioInterface);
        await microphone.ActivateAsync();

        // 計測時間を入力する
        var span = _view.InputSpan();

        // 調整値を取得する
        var specifyVolume = _view.InputDecibel();

        // 音声を再生する
        var recordingSettings = await _recordingSettingsRepository.LoadAsync();
        var mediaPlayer =_mediaPlayerProvider.Resolve(recordingSettings.IsEnableRemotePlaying);
        try
        {
            // 計測を開始する
            await Calibrate(audioInterface, mediaPlayer, microphone, specifyVolume, TimeSpan.FromSeconds(span));
        }
        finally
        {
            // リソースを停止・解放する。
            microphone.Deactivate();
        }
    }

    private async Task Calibrate(
        IAudioInterface audioInterface, 
        IMediaPlayer mediaPlayer, 
        IMicrophone microphone, 
        Decibel specifyVolume,
        TimeSpan span)
    {
        while (audioInterface.DefaultOutputLevel < VolumeLevel.Maximum)
        {
            CancellationTokenSource source = new();
            await mediaPlayer.PlayLoopingAsync(source.Token);
            // ReSharper disable once MethodSupportsCancellation
            await Task.Delay(TimeSpan.FromSeconds(1));
            try
            {
                // ボリュームを表示する
                _view.DisplayDefaultOutputLevel(audioInterface.DefaultOutputLevel);

                // 計測を開始する
                var meter = new InputLevelMeter(microphone);
                meter.StartMonitoring();

                // 計測完了を待機する
                // ReSharper disable once MethodSupportsCancellation
                await Task.Delay(span);

                // 計測結果を取得する
                var microphoneInputLevel = meter.StopMonitoring();
                _view.DisplayOutputVolume(microphoneInputLevel.Avg);

                if (specifyVolume < microphoneInputLevel.Avg)
                {
                    break;
                }

                var diff = (int)(Math.Ceiling((specifyVolume - microphoneInputLevel.Avg).AsPrimitive()) * 2.5);
                diff = diff == 0 ? 1 : diff;

                audioInterface.DefaultOutputLevel += new VolumeLevel(diff / 100f);
            }
            finally
            {
                source.Cancel();
            }
        }
    }
}