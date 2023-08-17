namespace Quietrum;

/// <summary>
/// リモートレコーダー
/// </summary>
public class RemoteRecorder : IRecorder
{
    /// <summary>
    /// HTTPクライアント
    /// </summary>
    private static readonly HttpClient HttpClient = new();

    /// <summary>
    /// 記録名
    /// </summary>
    private readonly string? _recordName;

    /// <summary>
    /// Settingリポジトリー
    /// </summary>
    private readonly ISettingsRepository _repository;

    /// <summary>
    /// インスタンスを生成する。
    /// </summary>
    /// <param name="recordName"></param>
    /// <param name="repository"></param>
    public RemoteRecorder(string? recordName, ISettingsRepository repository)
    {
        _recordName = recordName;
        _repository = repository;
    }

    /// <summary>
    /// 未実装。
    /// </summary>
    public IReadOnlyList<IMicrophoneRecorder> MicrophoneRecorders => throw new NotImplementedException();

    /// <summary>
    /// キャンセルするまで録音する。
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task StartAsync(CancellationToken token)
    {
        Settings settings = await _repository.LoadAsync();
        await HttpClient.GetAsync($"http://{settings.RecorderHost}:5000/Recorder/Recode/{_recordName}", token);

        token.Register(() =>
        {
            HttpClient
                // tokenに対してキャンセルが呼ばれた後なので、引数のtokenを渡すと停止が呼ばれないため警告を抑制する
                // ReSharper disable once MethodSupportsCancellation
                .GetAsync($"http://{settings.RecorderHost}:5000/Recorder/Stop")
                // ReSharper disable once MethodSupportsCancellation
                .Wait();
        });
    }

    /// <summary>
    /// 未実装。
    /// </summary>
    /// <param name="device"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public IMicrophoneRecorder GetLogger(IDevice device) => throw new NotImplementedException();
}