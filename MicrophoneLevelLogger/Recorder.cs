using CsvHelper;
using System.Globalization;

namespace MicrophoneLevelLogger;

/// <summary>
/// 記録対象のマイクに対するレコーダー
/// </summary>
public class Recorder : IRecorder
{
    /// <summary>
    /// ローカル保管ディレクトリの名称
    /// </summary>
    private static readonly DirectoryInfo RootDirectory = new("Record");
    /// <summary>
    /// サンプリング間隔における最大音量を時系列にCSVで記録するためのライター
    /// </summary>
    private readonly StreamWriter _maxDecibelLogger;
    /// <summary>
    /// 記録名
    /// </summary>
    private readonly string? _recordName;
    /// <summary>
    /// 記録ディレクトリ
    /// </summary>
    private readonly DirectoryInfo? _saveDirectory;
    /// <summary>
    /// IRecordSummaryリポジトリー
    /// </summary>
    private readonly IRecordSummaryRepository _recordSummaryRepository;

    /// <summary>
    /// インスタンスを生成する。
    /// </summary>
    /// <param name="audioInterface"></param>
    /// <param name="recordSummaryRepository"></param>
    /// <param name="recordName"></param>
    public Recorder(
        IAudioInterface audioInterface, IRecordSummaryRepository recordSummaryRepository, string? recordName = null)
        : this(recordSummaryRepository, recordName, audioInterface.GetMicrophones().ToArray())
    {
    }

    /// <summary>
    /// インスタンスを生成する。
    /// </summary>
    /// <param name="recordSummaryRepository"></param>
    /// <param name="recordName"></param>
    /// <param name="microphones"></param>
    public Recorder(IRecordSummaryRepository recordSummaryRepository, string? recordName = null,
        params IMicrophone[] microphones)
    {
        _recordSummaryRepository = recordSummaryRepository;
        _recordName = recordName;
        _saveDirectory =
            recordName is not null
                ? new DirectoryInfo(Path.Join(RootDirectory.FullName, $"{DateTime.Now:yyyy-MM-dd_HHmmss}_{recordName}"))
                : null;
        _saveDirectory?.Create();
        MicrophoneRecorders = microphones
            .Select(x => (IMicrophoneRecorder)new MicrophoneRecorder(x, _saveDirectory))
            .ToList();
        _maxDecibelLogger = _saveDirectory is not null
            ? File.CreateText(Path.Combine(_saveDirectory.FullName, "detail.csv"))
            : StreamWriter.Null;
    }

    /// <summary>
    /// マイク別レコーダー
    /// </summary>
    public IReadOnlyList<IMicrophoneRecorder> MicrophoneRecorders { get; }

    /// <summary>
    /// 記録を開始する。
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task StartAsync(CancellationToken token)
    {
        // 開始時刻
        var beginTime = DateTime.Now;
        // すべてのマイクのログインぐを開始する。
        await MicrophoneRecorders.ForEachAsync(x => x.StartAsync(token));

        // 非同期でロギングを実施する。
        var task = Task.Run(async () => await LoggingAsync(token), token);

        // キャンセル処理
        async void Cancel()
        {
            // ロギングタスクがキャンセルによって完全に停止するのを待機する。
            // ReSharper disable once MethodSupportsCancellation
            await task.WaitAsync(Timeout.InfiniteTimeSpan);

            if (_recordName is not null)
            {
                // 最小値、平均値、最大値をCSVファイルに出力する。
                var results = MicrophoneRecorders.Select((x, index) => new RecordResult(index + 1, x))
                    .ToList();
                await using var writer = new CsvWriter(File.CreateText(Path.Combine(_saveDirectory!.FullName, "summary.csv")), new CultureInfo("ja-JP", false));
                // ReSharper disable once MethodSupportsCancellation
                await writer.WriteRecordsAsync(results);

                // サマリーを出力する。
                var summary = new RecordSummary(_recordName!, beginTime, DateTime.Now, MicrophoneRecorders.Select(x => new MicrophoneRecordSummary(x.Microphone.Id, x.Microphone.Name, x.Min, x.Avg, x.Max)).ToList());
                await _recordSummaryRepository.SaveAsync(summary, _saveDirectory);
            }
        }

        // キャンセル処理を登録する。
        token.Register(Cancel);
    }

    /// <summary>
    /// 該当マイクのレコーダーを取得する。
    /// </summary>
    /// <param name="microphone"></param>
    /// <returns></returns>
    public IMicrophoneRecorder GetLogger(IMicrophone microphone) =>
        MicrophoneRecorders.Single(x => x.Microphone.Id == microphone.Id);

    /// <summary>
    /// 非同期でロギングする。
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
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
                await Task.Delay(MicrophoneRecorder.SamplingSpan, token);
            }
            catch (TaskCanceledException)
            {
            }
        }

        // ストリームをフラッシュして破棄する。
        await _maxDecibelLogger.FlushAsync();
    }

    /// <summary>
    /// 1行を記録する。
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// ヘッダーを記録する。
    /// </summary>
    /// <returns></returns>
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
        _maxDecibelLogger.Dispose();
    }
}