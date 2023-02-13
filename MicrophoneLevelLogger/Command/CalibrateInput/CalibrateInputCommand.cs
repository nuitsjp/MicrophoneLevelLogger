using MicrophoneLevelLogger.Domain;

namespace MicrophoneLevelLogger.Command.CalibrateInput;

public class CalibrateInputCommand : ICommand
{
    private const string DirectoryName = "Calibrate";

    private readonly IAudioInterfaceProvider _audioInterfaceProvider;
    private readonly ICalibrateInputView _view;
    private readonly IMediaPlayer _mediaPlayer;

    public CalibrateInputCommand(
        IAudioInterfaceProvider audioInterfaceProvider,
        ICalibrateInputView view,
        IMediaPlayer mediaPlayer)
    {
        _audioInterfaceProvider = audioInterfaceProvider;
        _view = view;
        _mediaPlayer = mediaPlayer;
    }

    public string Name => "Calibrate input      : マイクの入力レベルを調整する。";

    public async Task ExecuteAsync()
    {
        // すべてのマイクを取得する。
        using var audioInterface = _audioInterfaceProvider.Resolve();

        // 起動時情報を通知する。
        _view.NotifyMicrophonesInformation(audioInterface);

        // リファレンスマイクを選択する
        var reference = _view.SelectReference(audioInterface);

        // 調整対象のマイクを選択する
        var target = _view.SelectTarget(audioInterface, reference);

        // Recordingディレクトリを作成する
        if (Directory.Exists(DirectoryName))
        {
            Directory.Delete(DirectoryName, true);
        }

        Directory.CreateDirectory(DirectoryName);

        // 音声を再生する
        await _mediaPlayer.PlayLoopingAsync();
        try
        {
            // マイクを有効化する
            audioInterface.ActivateMicrophones();

            // 画面に入力レベルを通知する。
            //_view.StartNotifyMasterPeakValue(microphones);

            // マイクレベルを順番にキャリブレーションする
            Calibrate(reference, target);

            // 画面の入力レベル通知を停止する。
            //_view.StopNotifyMasterPeakValue();

            // キャリブレート結果を保存する
            MicrophoneCalibrationValue value = new(target.Id, target.Name, target.VolumeLevel);
            AudioInterfaceCalibrationValues values = await AudioInterfaceCalibrationValues.LoadAsync();
            values.Update(value);
            await AudioInterfaceCalibrationValues.SaveAsync(values);

            // 結果を表示する
            _view.NotifyCalibrated(values, target);


        }
        finally
        {
            await _mediaPlayer.StopAsync();
            audioInterface.DeactivateMicrophones();
        }
    }

    private void Calibrate(IMicrophone reference, IMicrophone target)
    {
        // ボリューム調整していくステップ
        VolumeLevel step = new(0.01f);

        Console.WriteLine(target);

        // ターゲットの入力レベルをMaxにする
        target.VolumeLevel = VolumeLevel.Maximum;

        // ターゲット側の入力レベルを少しずつ下げていきながら
        // リファレンスと同程度の音量になるように調整していく。
        var high = 1d;

        for (; VolumeLevel.Minimum < target.VolumeLevel; target.VolumeLevel -= step)
        {
            // レコーディング開始
            reference.StartRecording(DirectoryName);
            target.StartRecording(DirectoryName);

            Thread.Sleep(TimeSpan.FromSeconds(5));

            // レコーディング停止
            var referenceLevel = reference.StopRecording().PeakValues.Average();
            var targetLevel = target.StopRecording().PeakValues.Average();

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

            var diff = Math.Floor(Math.Abs(referenceLevel) - Math.Abs(targetLevel));
            step = new((float)(diff / 100));
            // 差がごく小さい場合、stepが0になってしまうので最小は0.01になるように調整する
            step = step == new VolumeLevel(0f) ? new(0.01f) : step;

            high = targetLevel;
        }
    }
}