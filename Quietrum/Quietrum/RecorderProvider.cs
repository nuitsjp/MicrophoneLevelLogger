namespace Quietrum;

/// <summary>
/// IRecorderプロバイダー
/// </summary>
public class RecorderProvider : IRecorderProvider
{
    /// <summary>
    /// Settingリポジトリー
    /// </summary>
    private readonly ISettingsRepository _repository;
    /// <summary>
    /// RecordSummaryリポジトリー
    /// </summary>
    private readonly IRecordSummaryRepository _recordSummaryRepository;
    /// <summary>
    /// IDetailRepositoryファクトリー
    /// </summary>
    private readonly IDetailRepositoryFactory _detailRepositoryFactory;

    /// <summary>
    /// インスタンスを生成する。
    /// </summary>
    /// <param name="repository"></param>
    /// <param name="recordSummaryRepository"></param>
    /// <param name="detailRepositoryFactory"></param>
    public RecorderProvider(
        ISettingsRepository repository, 
        IRecordSummaryRepository recordSummaryRepository, 
        IDetailRepositoryFactory detailRepositoryFactory)
    {
        _repository = repository;
        _recordSummaryRepository = recordSummaryRepository;
        _detailRepositoryFactory = detailRepositoryFactory;
    }

    /// <summary>
    /// ローカルのレコーダーを解決する。
    /// </summary>
    /// <param name="audioInterface"></param>
    /// <param name="recordName">nullの場合、計測はするが録音をローカルに保存しない。</param>
    /// <returns></returns>
    public IRecorder ResolveLocal(IAudioInterface audioInterface, string? recordName)
        => new Recorder(audioInterface, _recordSummaryRepository, _detailRepositoryFactory, recordName);

    /// <summary>
    /// 指定されたマイクを解決する。
    /// </summary>
    /// <param name="microphones"></param>
    /// <returns></returns>
    public IRecorder ResolveLocal(params IMicrophone[] microphones)
        => new Recorder(_recordSummaryRepository, _detailRepositoryFactory, null, microphones);

    /// <summary>
    /// リモートのレコーダーを解決する。
    /// </summary>
    /// <param name="recordName"></param>
    /// <returns></returns>
    public IRecorder ResolveRemote(string? recordName)
        => new RemoteRecorder(recordName, _repository);
}