namespace Quietrum;

/// <summary>
/// リモートメディアプレイヤー
/// </summary>
public class RemoteMediaPlayer : IMediaPlayer
{
    /// <summary>
    /// HTTPクライアント
    /// </summary>
    private static readonly HttpClient HttpClient = new();
    /// <summary>
    /// リモートホスト
    /// </summary>
    private readonly string _remoteHost;
    /// <summary>
    /// インスタンスを生成する。
    /// </summary>
    /// <param name="remoteHost"></param>
    public RemoteMediaPlayer(string remoteHost)
    {
        _remoteHost = remoteHost;
    }

    /// <summary>
    /// キャンセルするまでループ再生する。
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task PlayLoopingAsync(CancellationToken token)
    {
        await HttpClient.GetAsync($"http://{_remoteHost}:5000/Player/Play", token);
        token.Register(() =>
        {
            HttpClient
                // tokenに対してキャンセルが呼ばれた後なので、引数のtokenを渡すと停止が呼ばれないため警告を抑制する
                // ReSharper disable once MethodSupportsCancellation
                .GetAsync($"http://{_remoteHost}:5000/Player/Stop")
                // ReSharper disable once MethodSupportsCancellation
                .Wait();
        });
    }
}