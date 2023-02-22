namespace MicrophoneLevelLogger.Client.Controller.DisplayAudioInterface;

public class DisplayAudioInterfaceController : IController
{
    private readonly IMicrophoneView _view;
    private readonly IAudioInterfaceProvider _provider;

    public DisplayAudioInterfaceController(IMicrophoneView view, IAudioInterfaceProvider provider)
    {
        _view = view;
        _provider = provider;
    }

    public string Name => "Display audio interface";
    public string Description => "マイクやスピーカーの情報を表示します。";
    public async Task ExecuteAsync()
    {
        await _view.NotifyAudioInterfaceAsync(_provider.Resolve());
    }
}