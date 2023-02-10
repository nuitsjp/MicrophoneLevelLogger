using MicrophoneLevelLogger.Command;

namespace MicrophoneLevelLogger.Domain;

public interface IRecorder
{
    Task RecodeAsync();
    Task StopAsync();
}

public interface IRecorderProvider
{
    IRecorder ResolveLocal();
    IRecorder ResolveRemote();
}

public class RecorderProvider : IRecorderProvider
{
    private readonly IRecordView _view;
    private readonly IAudioInterfaceProvider _audioInterfaceProvider;

    public RecorderProvider(
        IRecordView view, 
        IAudioInterfaceProvider audioInterfaceProvider)
    {
        _view = view;
        _audioInterfaceProvider = audioInterfaceProvider;
    }

    public IRecorder ResolveLocal()
    {
        return new LocalRecorder(_view, _audioInterfaceProvider);
    }

    public IRecorder ResolveRemote()
    {
        return new RemoteRecorder();
    }
}