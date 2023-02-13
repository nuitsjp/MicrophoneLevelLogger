using MicrophoneLevelLogger.Domain;

namespace MicrophoneLevelLogger.Client.Command.DisplayMicrophones;

public class DisplayMicrophonesCommand : ICommand
{
    private readonly IMicrophoneView _view;
    private readonly IAudioInterfaceProvider _audioInterfaceProvider;

    public DisplayMicrophonesCommand(
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