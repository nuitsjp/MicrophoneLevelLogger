namespace MicrophoneLevelLogger;

public class RecorderProvider : IRecorderProvider
{
    private readonly IRecordingSettingsRepository _repository;
    private readonly IRecordSummaryRepository _recordSummaryRepository;

    public RecorderProvider(IRecordingSettingsRepository repository, IRecordSummaryRepository recordSummaryRepository)
    {
        _repository = repository;
        _recordSummaryRepository = recordSummaryRepository;
    }

    public IRecorder ResolveLocal(IAudioInterface audioInterface, string? recordName)
        => new Recorder(audioInterface, _recordSummaryRepository, recordName);

    public IRecorder ResolveLocal(params IMicrophone[] microphones)
        => new Recorder(_recordSummaryRepository, null, microphones);

    public IRecorder ResolveRemote(string? recordName)
        => new RemoteRecorder(recordName, _repository);
}