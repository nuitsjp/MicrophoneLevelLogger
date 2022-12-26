using MicrophoneLevelLogger.Domain;
using MicrophoneLevelLogger.View;

namespace MicrophoneLevelLogger.Command;

public class CalibrateCommand : ICommand
{
    private readonly IMicrophonesProvider _microphonesProvider;
    private readonly ICalibrateView _view;

    public CalibrateCommand(IMicrophonesProvider microphonesProvider, ICalibrateView view)
    {
        _microphonesProvider = microphonesProvider;
        _view = view;
    }

    public string Name => "Calibrate";

    public Task ExecuteAsync()
    {
        // すべてのマイクを取得する。
        using var microphones = _microphonesProvider.Resolve();

        // 起動時情報を通知する。
        _view.NotifyMicrophonesInformation(microphones);

        // リファレンスマイクを選択する
        var reference = _view.SelectReference(microphones);

        // Recordingディレクトリを作成する
        if (Directory.Exists(Name))
        {
            Directory.Delete(Name, true);
        }

        Directory.CreateDirectory(Name);

        // マイクを有効化する
        microphones.Activate();

        // 画面に入力レベルを通知する。
        _view.StartNotifyMasterPeakValue(microphones);

        // マイクレベルを順番にキャリブレーションする
        foreach (var microphone in microphones.Devices.Where(x => x != reference))
        {
            Calibrate(reference, microphone);
        }

        // 画面の入力レベル通知を停止する。
        _view.StopNotifyMasterPeakValue();

        _view.NotifyCalibrated(microphones);

        // マイクを無効化する
        microphones.Deactivate();

        return Task.CompletedTask;
    }

    private void Calibrate(IMicrophone reference, IMicrophone target)
    {
        // ボリューム調整していくステップ
        MasterVolumeLevelScalar step = new(0.005f);

        Console.WriteLine(target);

        // ターゲットの入力レベルをMaxにする
        target.MasterVolumeLevelScalar = MasterVolumeLevelScalar.Maximum;

        // ターゲット側の入力レベルを少しずつ下げていきながら
        // リファレンスと同程度の音量になるように調整していく。
        var high = 1d;
        for (; MasterVolumeLevelScalar.Minimum < target.MasterVolumeLevelScalar; target.MasterVolumeLevelScalar -= step)
        {
            // レコーディング開始
            reference.StartRecording(Name);
            target.StartRecording(Name);

            Thread.Sleep(TimeSpan.FromMilliseconds(500));

            // レコーディング停止
            var referenceLevel = reference.StopRecording().PeakValues.Average();
            var targetLevel = target.StopRecording().PeakValues.Average();

            if (targetLevel <= referenceLevel)
            {
                // キャリブレーション対象のレベルがリファレンスより小さくなったら調整を終了する

                // リファレンスより小さくなった際の値と、リファレンスより大きかった際の値を比較する
                // 小さくなった際の方が誤差が小さかった場合、
                if (!(referenceLevel - targetLevel < high - referenceLevel)) return;


                // 大きかった時(high)の方が誤差が小さかった場合、入力レベルをステップ分戻す
                if (target.MasterVolumeLevelScalar < MasterVolumeLevelScalar.Maximum)
                {
                    target.MasterVolumeLevelScalar += step;
                }

                return;
            }

            high = targetLevel;
        }
    }
}