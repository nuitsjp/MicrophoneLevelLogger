using System.CodeDom;
using CsvHelper;
using System.Globalization;
using FftSharp.Windows;

namespace MicrophoneLevelLogger;

public class Recorder : IRecorder
{
    private static readonly DirectoryInfo RootDirectory = new("Record");
    private readonly StreamWriter _maxDecibelLogger;
    private readonly DirectoryInfo? _saveDirectory;

    public Recorder(
        IAudioInterface audioInterface, 
        string? recordName = null)
        : this(recordName, audioInterface.Microphones.ToArray())
    {
    }

    public Recorder(
        string? recordName = null,
        params IMicrophone[] microphones)
    {
        _saveDirectory =
            recordName is not null
                ? new DirectoryInfo(Path.Join(RootDirectory.FullName, $"{DateTime.Now:yyyy-mm-dd_hhMMss}_{recordName}"))
                : null;
        _saveDirectory?.Create();
        MicrophoneRecorders = microphones
            .Select(x => (IMicrophoneRecorder)new MicrophoneRecorder(x, _saveDirectory))
            .ToList();
        _maxDecibelLogger = _saveDirectory is not null
            ? File.CreateText(Path.Combine(_saveDirectory.FullName, "detail.csv"))
            : StreamWriter.Null;
    }

    public IReadOnlyList<IMicrophoneRecorder> MicrophoneRecorders { get; }

    public async Task StartAsync(CancellationToken token)
    {
        // すべてのマイクのログインぐを開始する。
        await MicrophoneRecorders.ForEachAsync(x => x.StartAsync(token));

        // 非同期でロギングを実施する。
        var task = Task.Run(async () => await LoggingAsync(token), token);

        // キャンセル処理ハンドラーを登録する。
        token.Register(() =>
        {
            // ReSharper disable once MethodSupportsCancellation
            task.Wait();

            if (_saveDirectory is not null)
            {
                // 最小値、平均値、最大値をテキストファイルに出力する。
                var results = MicrophoneRecorders
                    .Select((x, index) => new RecordResult(index + 1, x))
                    .ToList();
                using var writer =
                    new CsvWriter(
                        File.CreateText(Path.Combine(_saveDirectory.FullName, "summary.csv")),
                        new CultureInfo("ja-JP", false));
                writer.WriteRecords(results);
            }
        });
    }

    public IMicrophoneRecorder GetLogger(IMicrophone microphone) =>
        MicrophoneRecorders.Single(x => x.Microphone.Id == microphone.Id);

    private async Task LoggingAsync(CancellationToken token)
    {
        // CSVのヘッダーを出力する。
        await WriteHeaderAsync();

        // キャンセルされるまで繰り返す。
        while (token.IsCancellationRequested is false)
        {
            // レコードを出力する。
            await WriteRecordAsync();
            try
            {
                // サンプリング間隔待機する。
                await Task.Delay(IMicrophone.SamplingMilliseconds, token);
            }
            catch (TaskCanceledException)
            {
            }
        }

        // ストリームをフラッシュして破棄する。
        await _maxDecibelLogger.FlushAsync();
    }

    private async Task WriteRecordAsync()
    {
        await _maxDecibelLogger.WriteAsync($"{DateTime.Now:yyyy/MM/dd hh:mm:ss.fff}");
        foreach (var microphoneLogger in MicrophoneRecorders)
        {
            await _maxDecibelLogger.WriteAsync(",");
            _maxDecibelLogger.Write(microphoneLogger.Max);
        }

        await _maxDecibelLogger.WriteLineAsync();
    }

    private async Task WriteHeaderAsync()
    {
        await _maxDecibelLogger.WriteAsync("時刻");
        foreach (var microphoneLogger in MicrophoneRecorders)
        {
            await _maxDecibelLogger.WriteAsync(",");
            await _maxDecibelLogger.WriteAsync(microphoneLogger.Microphone.Name);
        }

        await _maxDecibelLogger.WriteLineAsync();
    }

    public void Dispose()
    {
        foreach (var logger in MicrophoneRecorders)
        {
            logger.DisposeQuiet();
        }
        _maxDecibelLogger.Dispose();
    }
}