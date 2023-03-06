namespace MicrophoneLevelLogger.Client.Controller.CalibrateOutput;

/// <summary>
/// スピーカーを調整する。
/// </summary>
public class CalibrateOutputController : IController
{
    /// <summary>
    /// IAudioInterfaceプロバイダー
    /// </summary>
    private readonly IAudioInterfaceProvider _audioInterfaceProvider;
    /// <summary>
    /// ビュー
    /// </summary>
    private readonly ICalibrateOutputView _view;
    /// <summary>
    /// IRecorderプロバイダー
    /// </summary>
    private readonly IRecorderProvider _recorderProvider;
    /// <summary>
    /// Settingリポジトリー
    /// </summary>
    private readonly ISettingsRepository _settingsRepository;

    /// <summary>
    /// インスタンスを生成する。
    /// </summary>
    /// <param name="audioInterfaceProvider"></param>
    /// <param name="view"></param>
    /// <param name="settingsRepository"></param>
    /// <param name="recorderProvider"></param>
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

    /// <summary>
    /// 名称
    /// </summary>
    public string Name => "Calibrate output";
    /// <summary>
    /// 概要
    /// </summary>
    public string Description => "スピーカーの出力レベルを調整する。";

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
        IMediaPlayer mediaPlayer = recordingSettings.IsEnableRemotePlaying
            ? new RemoteMediaPlayer(recordingSettings.MediaPlayerHost)
            : new MediaPlayer(await audioInterface.GetSpeakerAsync());
        // 計測を開始する
        await Calibrate(audioInterface, mediaPlayer, microphone, specifyVolume, TimeSpan.FromSeconds(span));
    }

    /// <summary>
    /// 調整する。
    /// </summary>
    /// <param name="audioInterface"></param>
    /// <param name="mediaPlayer"></param>
    /// <param name="microphone"></param>
    /// <param name="specifyVolume"></param>
    /// <param name="span"></param>
    /// <returns></returns>
    private async Task Calibrate(
        IAudioInterface audioInterface, 
        IMediaPlayer mediaPlayer, 
        IMicrophone microphone, 
        Decibel specifyVolume,
        TimeSpan span)
    {
        var speaker = await audioInterface.GetSpeakerAsync();
        while (speaker.VolumeLevel < VolumeLevel.Maximum)
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
                _view.DisplaySpeakerVolumeLevel(speaker.VolumeLevel);

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

            if (100 < speaker.VolumeLevel.AsPrimitive() * 100 + diff)
            {
                // 最大値を超えてしまう場合、最大値に設定する
                speaker.VolumeLevel = VolumeLevel.Maximum;
            }
            else
            {
                speaker.VolumeLevel += new VolumeLevel(diff / 100f);
            }
        }
    }
}