namespace MicrophoneLevelLogger;

public class AudioInterfaceLoggerProvider : IAudioInterfaceLoggerProvider
{
    private readonly IRecordingSettingsRepository _repository;

    public AudioInterfaceLoggerProvider(IRecordingSettingsRepository repository)
    {
        _repository = repository;
    }

    public IAudioInterfaceLogger ResolveLocal(IAudioInterface audioInterface, string? recordName)
        => new AudioInterfaceLogger(audioInterface, recordName);

    public IAudioInterfaceLogger ResolveRemote(string? recordName)
        => new RemoteAudioInterfaceLogger(recordName, _repository);
}