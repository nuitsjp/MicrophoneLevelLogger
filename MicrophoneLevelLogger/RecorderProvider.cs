namespace MicrophoneLevelLogger;

public class RecorderProvider : IRecorderProvider
{
    private readonly IAudioInterfaceProvider _audioInterfaceProvider;
    private readonly IRecordingSettingsRepository _repository;

    public RecorderProvider(
        IAudioInterfaceProvider audioInterfaceProvider, 
        IRecordingSettingsRepository repository)
    {
        _audioInterfaceProvider = audioInterfaceProvider;
        _repository = repository;
    }

    public IRecorder ResolveLocal()
    {
        return new Recorder(_audioInterfaceProvider);
    }

    public IRecorder ResolveRemote()
    {
        return new RemoteRecorder(_repository);
    }
}