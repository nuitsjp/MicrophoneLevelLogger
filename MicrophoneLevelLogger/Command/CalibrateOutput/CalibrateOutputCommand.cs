using MicrophoneLevelLogger.Domain;

namespace MicrophoneLevelLogger.Command.CalibrateOutput;

public class CalibrateOutputCommand : ICommand
{
    private readonly IAudioInterfaceProvider _audioInterfaceProvider;
    private readonly ICalibrateOutputView _view;
    private readonly IMediaPlayerProvider _mediaPlayerProvider;

    public CalibrateOutputCommand(
        IAudioInterfaceProvider audioInterfaceProvider, 
        ICalibrateOutputView view, 
        IMediaPlayerProvider mediaPlayerProvider)
    {
        _audioInterfaceProvider = audioInterfaceProvider;
        _view = view;
        _mediaPlayerProvider = mediaPlayerProvider;
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
        var recordingSettings = await Domain.RecordingSettings.LoadAsync();
        var mediaPlayer =
            recordingSettings.IsEnableRemotePlaying
                ? _mediaPlayerProvider.ResolveRemoteService()
                : _mediaPlayerProvider.ResolveLocaleService();
        await mediaPlayer.PlayLoopingAsync();

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
        double specifyVolume,
        TimeSpan span)
    {
        while (audioInterface.DefaultOutputLevel < VolumeLevel.Maximum)
        {
            await mediaPlayer.PlayLoopingAsync();
            await Task.Delay(TimeSpan.FromSeconds(1));
            try
            {
                // ボリュームを表示する
                _view.DisplayDefaultOutputLevel(audioInterface.DefaultOutputLevel);

                // 計測を開始する
                var meter = new InputLevelMeter(microphone);
                meter.StartMonitoring();

                // 計測完了を待機する
                await Task.Delay(span);

                // 計測結果を取得する
                var microphoneInputLevel = meter.StopMonitoring();
                _view.DisplayOutputVolume(microphoneInputLevel.Avg);

                if (specifyVolume < microphoneInputLevel.Avg)
                {
                    break;
                }

                var diff = (int)(Math.Ceiling(specifyVolume - microphoneInputLevel.Avg) * 2.5);
                diff = diff == 0 ? 1 : diff;

                audioInterface.DefaultOutputLevel += new VolumeLevel(diff / 100f);
            }
            finally
            {
                await mediaPlayer.StopAsync();
            }
        }
    }
}