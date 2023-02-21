namespace MicrophoneLevelLogger;

public class RemoteMediaPlayer : IMediaPlayer
{
    private static readonly HttpClient HttpClient = new();
    private readonly ISettingsRepository _repository;

    public RemoteMediaPlayer(ISettingsRepository repository)
    {
        _repository = repository;
    }

    public async Task PlayLoopingAsync(CancellationToken token)
    {
        Settings settings = await _repository.LoadAsync();
        await HttpClient.GetAsync($"http://{settings.MediaPlayerHost}:5000/Player/Play", token);
        token.Register(() =>
        {
            HttpClient
                // tokenに対してキャンセルが呼ばれた後なので、引数のtokenを渡すと停止が呼ばれないため警告を抑制する
                // ReSharper disable once MethodSupportsCancellation
                .GetAsync($"http://{settings.MediaPlayerHost}:5000/Player/Stop")
                // ReSharper disable once MethodSupportsCancellation
                .Wait();
        });
    }

    public void Dispose()
    {
    }
}