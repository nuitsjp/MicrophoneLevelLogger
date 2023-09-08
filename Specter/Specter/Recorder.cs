namespace Specter;

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
    /// マイク別のサンプリング間隔ごとの最大音量を記録するライター
    /// </summary>
    private readonly IDetailRepositoryFactory _detailRepositoryFactory;

    /// <summary>
    /// インスタンスを生成する。
    /// </summary>
    /// <param name="audioInterface"></param>
    /// <param name="recordSummaryRepository"></param>
    /// <param name="detailRepositoryFactory"></param>
    /// <param name="recordName"></param>
    public Recorder(
        IAudioInterface audioInterface, 
        IRecordSummaryRepository recordSummaryRepository, 
        IDetailRepositoryFactory detailRepositoryFactory, 
        string? recordName = null)
        : this(recordSummaryRepository, detailRepositoryFactory, recordName, audioInterface.Devices.ToArray())
    {
    }

    /// <summary>
    /// インスタンスを生成する。
    /// </summary>
    /// <param name="recordSummaryRepository"></param>
    /// <param name="detailRepositoryFactory"></param>
    /// <param name="recordName"></param>
    /// <param name="microphones"></param>
    public Recorder(
        IRecordSummaryRepository recordSummaryRepository, 
        IDetailRepositoryFactory detailRepositoryFactory, 
        string? recordName = null,
        params IDevice[] microphones)
    {
        _recordSummaryRepository = recordSummaryRepository;
        _detailRepositoryFactory = detailRepositoryFactory;
        _recordName = recordName;
        _saveDirectory =
            recordName is not null
                ? new DirectoryInfo(Path.Join(RootDirectory.FullName, $"{DateTime.Now:yyyy-MM-dd_HHmmss}_{recordName}"))
                : null;
        _saveDirectory?.Create();
        MicrophoneRecorders = microphones
            .Select(x => (IMicrophoneRecorder)new MicrophoneRecorder(x, _saveDirectory))
            .ToList();
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
        var task = Task.Run(async () =>
        {
            // _saveDirectoryがnullの場合、Nullデバイスを指定して書き捨てする。
            using var detailWriter = _detailRepositoryFactory.Create(
                _saveDirectory is not null
                    ? File.CreateText(Path.Combine(_saveDirectory.FullName, "detail.csv"))
                    : StreamWriter.Null);
            await detailWriter.WriteHeaderAsync(MicrophoneRecorders);

            // キャンセルされるまで繰り返す。
            while (token.IsCancellationRequested is false)
            {
                // レコードを出力する。
                await detailWriter.WriteRecordAsync(MicrophoneRecorders);
                try
                {
                    // サンプリング間隔待機する。
                    await Task.Delay(MicrophoneRecorder.SamplingSpan, token);
                }
                catch (TaskCanceledException)
                {
                }
            }

            // CSVのヘッダーを出力する。
        }, token);

        // キャンセル処理を登録する。
        // ReSharper disable once AsyncVoidLambda
        token.Register(async () =>
        {
            // ロギングタスクがキャンセルによって完全に停止するのを待機する。
            // ReSharper disable once MethodSupportsCancellation
            await task.WaitAsync(Timeout.InfiniteTimeSpan);

            if (_recordName is null) return;

            // サマリーを出力する。
            var summary = new RecordSummary(
                _recordName, 
                beginTime, 
                DateTime.Now, 
                MicrophoneRecorders
                    .Select(x => new MicrophoneRecordSummary(x.Device.Id, x.Device.Name, x.Min, x.Avg, x.Max))
                    .ToList());
            await _recordSummaryRepository.SaveAsync(summary, _saveDirectory!);
        });
    }

    /// <summary>
    /// 該当マイクのレコーダーを取得する。
    /// </summary>
    /// <param name="device"></param>
    /// <returns></returns>
    public IMicrophoneRecorder GetLogger(IDevice device) =>
        MicrophoneRecorders.Single(x => x.Device.Id == device.Id);
}