namespace MicrophoneLevelLogger;

public class RemoteMediaPlayer : IMediaPlayer
{
    private static readonly HttpClient HttpClient = new();
    private readonly string _remoteHost;
    public RemoteMediaPlayer(string remoteHost)
    {
        _remoteHost = remoteHost;
    }

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

    public void Dispose()
    {
    }
}