namespace MicrophoneLevelLogger;

public class RecorderProvider : IRecorderProvider
{
    private readonly IRecordingSettingsRepository _repository;

    public RecorderProvider(IRecordingSettingsRepository repository)
    {
        _repository = repository;
    }

    public IRecorder ResolveLocal(IAudioInterface audioInterface, string? recordName)
        => new Recorder(audioInterface, recordName);

    public IRecorder ResolveLocal(params IMicrophone[] microphones)
        => new Recorder(null, microphones);

    public IRecorder ResolveRemote(string? recordName)
        => new RemoteRecorder(recordName, _repository);
}