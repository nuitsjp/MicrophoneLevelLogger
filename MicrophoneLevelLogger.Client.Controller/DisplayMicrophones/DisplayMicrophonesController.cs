namespace MicrophoneLevelLogger.Client.Controller.DisplayMicrophones;

public class DisplayMicrophonesController : IController
{
    private readonly IMicrophoneView _view;
    private readonly IAudioInterfaceProvider _audioInterfaceProvider;

    public DisplayMicrophonesController(
        IMicrophoneView view,
        IAudioInterfaceProvider audioInterfaceProvider)
    {
        _view = view;
        _audioInterfaceProvider = audioInterfaceProvider;
    }

    public string Name => "Display microphones  : マイクの一覧を表示する。";
    public Task ExecuteAsync()
    {
        _view.NotifyMicrophonesInformation(_audioInterfaceProvider.Resolve());
        return Task.CompletedTask;
    }
}