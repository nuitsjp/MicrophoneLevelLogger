using MicrophoneLevelLogger.Domain;

namespace MicrophoneLevelLogger.Client.Command.MonitorVolume;

public class MonitorVolumeCommand : ICommand
{
    private readonly IAudioInterfaceProvider _audioInterfaceProvider;
    private readonly IMonitorVolumeView _view;

    public MonitorVolumeCommand(IAudioInterfaceProvider audioInterfaceProvider, IMonitorVolumeView view)
    {
        _audioInterfaceProvider = audioInterfaceProvider;
        _view = view;
    }

    public string Name => "Monitor volume       : マイクの入力をモニターする。データの保存は行わない。";

    public Task ExecuteAsync()
    {
        using var audioInterface = _audioInterfaceProvider.Resolve();

        audioInterface.ActivateMicrophones();
        try
        {
            _view.NotifyDetailMessage();
            _view.StartNotifyMasterPeakValue(audioInterface);

            _view.WaitToBeStopped();

            _view.StopNotifyMasterPeakValue();

            return Task.CompletedTask;
        }
        finally
        {
            audioInterface.DeactivateMicrophones();
        }
    }
}