using System.Globalization;
using CsvHelper;
using MicrophoneLevelLogger.Command;

namespace MicrophoneLevelLogger.Domain;

public class LocalRecorder : IRecorder
{
    public const string RecordDirectoryName = "Record";

    private readonly IRecordView _view;
    private readonly IAudioInterface _audioInterface;

    private CancellationTokenSource? _cancellationTokenSource;
    private string _saveDirectory = string.Empty;

    public LocalRecorder(
        IRecordView view,
        IAudioInterfaceProvider audioInterfaceProvider)
    {
        _view = view;
        _audioInterface = audioInterfaceProvider.Resolve();
    }

    public Task RecodeAsync()
    {
        // 起動時情報を通知する。
        _view.NotifyMicrophonesInformation(_audioInterface);

        // マイクを有効化する
        _audioInterface.ActivateMicrophones();

        _saveDirectory = Path.Combine(RecordDirectoryName, DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"));
        Directory.CreateDirectory(_saveDirectory);

        // キャプチャーを開始する。
        _audioInterface.StartRecording(_saveDirectory);

        // 画面に入力レベルを通知する。
        _view.StartNotifyMasterPeakValue(_audioInterface);

        // CSVへ出力を開始する。
        _cancellationTokenSource = new();
        WriteMaximumDecibels(_saveDirectory, _cancellationTokenSource.Token);

        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        if (_cancellationTokenSource is null)
        {
            return Task.CompletedTask;
        }

        // CSV出力へキャンセルを通知する。
        _cancellationTokenSource.Cancel();

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
                File.CreateText(Path.Combine(_saveDirectory, "summary.csv")),
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