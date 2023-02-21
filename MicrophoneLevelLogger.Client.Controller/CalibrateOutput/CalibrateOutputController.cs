using System.Diagnostics.Metrics;
using System.Security.Cryptography;
using MicrophoneLevelLogger.Client.Controller.DisplayCalibrates;

namespace MicrophoneLevelLogger.Client.Controller.CalibrateOutput;

public class CalibrateOutputController : IController
{
    private readonly IAudioInterfaceProvider _audioInterfaceProvider;
    private readonly ICalibrateOutputView _view;
    private readonly IRecorderProvider _recorderProvider;
    private readonly ISettingsRepository _settingsRepository;

    public CalibrateOutputController(
        IAudioInterfaceProvider audioInterfaceProvider, 
        ICalibrateOutputView view, 
        ISettingsRepository settingsRepository, 
        IRecorderProvider recorderProvider)
    {
        _audioInterfaceProvider = audioInterfaceProvider;
        _view = view;
        _settingsRepository = settingsRepository;
        _recorderProvider = recorderProvider;
    }

    public string Name => "Calibrate output     : スピーカーの出力レベルを調整する。";
    public async Task ExecuteAsync()
    {
        // マイクを選択し、有効化する
        var audioInterface = _audioInterfaceProvider.Resolve();
        var microphone = _view.SelectMicrophone(audioInterface);

        // 計測時間を入力する
        var span = _view.InputSpan();

        // 調整値を取得する
        var specifyVolume = _view.InputDecibel();

        // 音声を再生する
        var recordingSettings = await _settingsRepository.LoadAsync();
        var mediaPlayer =audioInterface.GetMediaPlayer(recordingSettings.IsEnableRemotePlaying);
        // 計測を開始する
        await Calibrate(audioInterface, mediaPlayer, microphone, specifyVolume, TimeSpan.FromSeconds(span));
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
            var recorder = _recorderProvider.ResolveLocal(microphone);

            // 音源を再生する。
            CancellationTokenSource source = new();
            await mediaPlayer.PlayLoopingAsync(source.Token);
            // 無音部が発生するため、冒頭部を聞き飛ばす。
            await Task.Delay(TimeSpan.FromSeconds(1), source.Token);
            try
            {
                // ボリュームを表示する
                _view.DisplayDefaultOutputLevel(audioInterface.DefaultOutputLevel);

                // 計測を開始する
                await recorder.StartAsync(source.Token);

                // 計測完了を待機する
                _view.Wait(span);
            }
            finally
            {
                source.Cancel();
            }

            // 計測結果を取得する
            var decibel = recorder.MicrophoneRecorders.Single().Avg;
            _view.DisplayOutputVolume(decibel);

            if (specifyVolume < decibel)
            {
                break;
            }

            var diff = (int)(Math.Ceiling((specifyVolume - decibel).AsPrimitive()) * 2.5);
            // 最低でも1は上げる
            diff = diff == 0 ? 1 : diff;

            if (1 < audioInterface.DefaultOutputLevel.AsPrimitive() * 100 + diff)
            {
                // 最大値を超えてしまう場合、最大値に設定する
                audioInterface.DefaultOutputLevel = VolumeLevel.Maximum;
            }
            else
            {
                audioInterface.DefaultOutputLevel += new VolumeLevel(diff / 100f);
            }
        }
    }
}