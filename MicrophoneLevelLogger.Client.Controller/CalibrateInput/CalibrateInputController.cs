namespace MicrophoneLevelLogger.Client.Controller.CalibrateInput;

/// <summary>
/// ２つのマイクの入力音量が、同程度になるように調整する
/// </summary>
public class CalibrateInputController : IController
{
    private readonly ICalibrateInputView _view;
    private readonly IAudioInterfaceProvider _audioInterfaceProvider;
    private readonly IRecorderProvider _recorderProvider;
    private readonly IAudioInterfaceCalibrationValuesRepository _audioInterfaceCalibrationValuesRepository;

    public CalibrateInputController(
        ICalibrateInputView view,
        IAudioInterfaceProvider audioInterfaceProvider,
        IAudioInterfaceCalibrationValuesRepository audioInterfaceCalibrationValuesRepository, 
        IRecorderProvider recorderProvider)
    {
        _audioInterfaceProvider = audioInterfaceProvider;
        _view = view;
        _audioInterfaceCalibrationValuesRepository = audioInterfaceCalibrationValuesRepository;
        _recorderProvider = recorderProvider;
    }

    /// <summary>
    /// コマンド名称を取得する。
    /// </summary>
    public string Name => "Calibrate input";

    public string Description => "マイクの入力レベルを調整する。";

    public async Task ExecuteAsync()
    {
        // すべてのマイクを取得する。
        var audioInterface = _audioInterfaceProvider.Resolve();

        // 起動時情報を通知する。
        await _view.NotifyAudioInterfaceAsync(audioInterface);

        // リファレンスマイクを選択する
        var reference = _view.SelectReference(audioInterface);

        // 調整対象のマイクを選択する
        var target = _view.SelectTarget(audioInterface, reference);

        // マイクレベルを順番にキャリブレーションする
        var mediaPlayer = new MediaPlayer(await audioInterface.GetSpeakerAsync());
        await Calibrate(reference, target, mediaPlayer);

        // キャリブレート結果を保存する
        MicrophoneCalibrationValue value = new(target.Id, target.Name, target.VolumeLevel);
        AudioInterfaceCalibrationValues values = await _audioInterfaceCalibrationValuesRepository.LoadAsync();
        values.Update(value);
        await _audioInterfaceCalibrationValuesRepository.SaveAsync(values);

        // 結果を表示する
        _view.NotifyCalibrated(values, target);
    }

    private async Task Calibrate(IMicrophone reference, IMicrophone target, IMediaPlayer mediaPlayer)
    {
        // ボリューム調整していくステップ
        VolumeLevel step = new(0.01f);

        // ターゲットの入力レベルをMaxにする
        target.VolumeLevel = VolumeLevel.Maximum;

        // ターゲット側の入力レベルを少しずつ下げていきながら
        // リファレンスと同程度の音量になるように調整していく。
        Decibel high = Decibel.Max;
        for (; VolumeLevel.Minimum < target.VolumeLevel; target.VolumeLevel -= step)
        {
            CancellationTokenSource source = new();
            var recorder = _recorderProvider.ResolveLocal(reference, target);
            try
            {
                // 音声を再生と、レコーディングを開始する。
                await mediaPlayer.PlayLoopingAsync(source.Token);
                await recorder.StartAsync(source.Token);

                _view.Wait(TimeSpan.FromSeconds(5));
            }
            finally
            {
                source.Cancel();
            }

            // レコーディング停止
            var referenceLevel = recorder.GetLogger(reference).Avg;
            var targetLevel = recorder.GetLogger(target).Avg;

            _view.NotifyProgress(reference, referenceLevel, target, targetLevel);

            if (targetLevel <= referenceLevel)
            {
                // キャリブレーション対象のレベルがリファレンスより小さくなったら調整を終了する

                // リファレンスより小さくなった際の値と、リファレンスより大きかった際の値を比較する
                // 小さくなった際の方が誤差が小さかった場合、
                if (!(referenceLevel - targetLevel < high - referenceLevel)) return;

                // 大きかった時(high)の方が誤差が小さかった場合、入力レベルをステップ分戻す
                if (target.VolumeLevel < VolumeLevel.Maximum)
                {
                    target.VolumeLevel += step;
                }

                return;
            }

            var diff = Math.Floor(Math.Abs(referenceLevel.AsPrimitive()) - Math.Abs(targetLevel.AsPrimitive()));
            step = new((float)(diff / 100));
            // 差がごく小さい場合、stepが0になってしまうので最小は0.01になるように調整する
            step = step == new VolumeLevel(0f) ? new(0.01f) : step;

            high = targetLevel;
        }
    }
}