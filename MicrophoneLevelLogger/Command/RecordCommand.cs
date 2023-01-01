using System.Diagnostics;
using System.Globalization;
using CsvHelper;
using FftSharp.Windows;
using FluentTextTable;
using MicrophoneLevelLogger.Domain;

namespace MicrophoneLevelLogger.Command;

public class RecordCommand : ICommand
{
    public const string RecordDirectoryName = "Record";

    private readonly IRecordView _view;
    private readonly IAudioInterface _audioInterface;

    public RecordCommand(IMicrophonesProvider microphonesProvider, IRecordView view)
    {
        _view = view;
        _audioInterface = microphonesProvider.Resolve();
    }

    public string Name => "Record      : マイクの入力をキャプチャーし保存する。";


    public Task ExecuteAsync()
    {
        // 起動時情報を通知する。
        _view.NotifyMicrophonesInformation(_audioInterface);

        // マイクを有効化する
        _audioInterface.ActivateMicrophones();

        var saveDirectory = Path.Combine(RecordDirectoryName, DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"));
        Directory.CreateDirectory(saveDirectory);

        // キャプチャーを開始する。
        _audioInterface.StartRecording(saveDirectory);

        // 画面に入力レベルを通知する。
        _view.StartNotifyMasterPeakValue(_audioInterface);

        // CSVへ出力を開始する。
        CancellationTokenSource cancellationTokenSource = new();
        WriteMaximumDecibels(saveDirectory, cancellationTokenSource.Token);

        // Enterが押下されるまで待機する。
        Console.ReadLine();

        // CSV出力へキャンセルを通知する。
        cancellationTokenSource.Cancel();

        // 画面の入力レベル通知を停止する。
        _view.StopNotifyMasterPeakValue();

        // キャプチャーを停止する。
        var peakValues = _audioInterface.StopRecording();

        // マイクを無効化する。
        _audioInterface.DeactivateMicrophones();

        // 最小値、平均値、最大値をテキストファイルに出力する。
        var results = peakValues
            .Select((x, index) => new RecordResult(index + 1, x))
            .ToList();
        using var writer =
            new CsvWriter(
                File.CreateText(Path.Combine(saveDirectory, "summary.csv")),
                new CultureInfo("ja-JP", false));
        writer.WriteRecords(results);

        // 結果を通知する
        _view.NotifyResult(results);

        return Task.CompletedTask;
    }

    private void WriteMaximumDecibels(string saveDirectory, CancellationToken token)
    {
        Task.Run(async () =>
        {
            await using var writer = File.CreateText(Path.Combine(saveDirectory, "detail.csv"));

            await writer.WriteAsync("時刻");
            foreach (var microphone in _audioInterface.Microphones)
            {
                await writer.WriteAsync(",");
                await writer.WriteAsync(microphone.Name);
            }
            await writer.WriteLineAsync();

            while (token.IsCancellationRequested is false)
            {
                await writer.WriteAsync($"{DateTime.Now:yyyy/MM/dd hh:mm:ss.fff}");
                foreach (var microphone in _audioInterface.Microphones)
                {
                    await writer.WriteAsync(",");
                    writer.Write(microphone.LatestWaveInput.MaximumDecibel);
                }
                await writer.WriteLineAsync();
                await Task.Delay(IMicrophone.SamplingMilliseconds, token);
            }

            await writer.FlushAsync();
        }, token);
    }
}