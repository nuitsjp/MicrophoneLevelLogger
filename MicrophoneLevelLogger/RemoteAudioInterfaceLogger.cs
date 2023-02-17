using System.Net.Http;
using System.Xml.Linq;

namespace MicrophoneLevelLogger;

public class RemoteAudioInterfaceLogger : IAudioInterfaceLogger
{
    private static readonly HttpClient HttpClient = new();

    private readonly string? _recordName;

    private readonly IRecordingSettingsRepository _repository;

    public RemoteAudioInterfaceLogger(string? recordName, IRecordingSettingsRepository repository)
    {
        _recordName = recordName;
        _repository = repository;
    }

    public IReadOnlyList<IMicrophoneLogger> MicrophoneLoggers => throw new NotImplementedException();

    public async Task StartAsync(CancellationToken token)
    {
        RecordingSettings settings = await _repository.LoadAsync();
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

    public IMicrophoneLogger GetLogger(IMicrophone microphone) => throw new NotImplementedException();


    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}