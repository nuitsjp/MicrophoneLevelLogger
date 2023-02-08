using System.Diagnostics;
using System.Globalization;
using CsvHelper;
using FftSharp.Windows;
using FluentTextTable;
using MicrophoneLevelLogger.Domain;

namespace MicrophoneLevelLogger.Command;

public class MeasureInputLevelCommand : ICommand
{
    public const string RecordDirectoryName = "Record";

    private readonly IMeasureInputLevelView _view;
    private readonly IAudioInterface _audioInterface;

    public MeasureInputLevelCommand(IAudioInterfaceProvider audioInterfaceProvider, IMeasureInputLevelView view)
    {
        _view = view;
        _audioInterface = audioInterfaceProvider.Resolve();
    }

    public string Name => "Measure input : 指定のマイクの入力レベルを計測する。";


    public async Task ExecuteAsync()
    {
        // 起動時情報を通知する。
        _view.NotifyMicrophonesInformation(_audioInterface);

        // リファレンスマイクを選択する
        var microphone = _view.SelectMicrophone(_audioInterface);

        // マイクを有効化する
        _audioInterface.ActivateMicrophones();

        var saveDirectory = Path.Combine(RecordDirectoryName, DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"));
        Directory.CreateDirectory(saveDirectory);

        // キャプチャーを開始する。
        _audioInterface.StartRecording(saveDirectory);

        // 画面に入力レベルを通知する。
        _view.StartNotifyMasterPeakValue(_audioInterface);

        // Enterが押下されるまで待機する。
        Console.ReadLine();

        // 画面の入力レベル通知を停止する。
        _view.StopNotifyMasterPeakValue();

        // キャプチャーを停止する。
        var peakValues = _audioInterface.StopRecording();

        // マイクを無効化する。
        _audioInterface.DeactivateMicrophones();

        // 最小値、平均値、最大値をテキストファイルに出力する。
        var masterPeakValues = peakValues.Single(x => x.Microphone.Id == microphone.Id);
        RecordResult recordResult = new(0, masterPeakValues);
        MicrophoneInputLevel microphoneInputLevel =
            new(
                microphone.Id,
                microphone.Name,
                recordResult.Min,
                recordResult.Avg,
                recordResult.Median,
                recordResult.Max);

        // 計測結果リストを更新する
        AudioInterfaceInputLevels inputLevels = await AudioInterfaceInputLevels.LoadAsync();
        inputLevels.Update(microphoneInputLevel);
        await AudioInterfaceInputLevels.SaveAsync(inputLevels);

        // 結果を通知する
        _view.NotifyResult(inputLevels);
    }
}