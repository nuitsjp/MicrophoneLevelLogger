namespace MicrophoneLevelLogger.Domain;

public class RecorderProvider : IRecorderProvider
{
    private readonly IAudioInterfaceProvider _audioInterfaceProvider;

    public RecorderProvider(IAudioInterfaceProvider audioInterfaceProvider)
    {
        _audioInterfaceProvider = audioInterfaceProvider;
    }

    public IRecorder ResolveLocal()
    {
        return new Recorder(_audioInterfaceProvider);
    }

    public IRecorder ResolveRemote()
    {
        return new RemoteRecorder();
    }
}