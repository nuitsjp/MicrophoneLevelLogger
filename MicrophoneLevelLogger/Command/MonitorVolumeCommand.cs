using MicrophoneLevelLogger.Domain;

namespace MicrophoneLevelLogger.Command;

public class MonitorVolumeCommand : ICommand
{
    private readonly IAudioInterfaceProvider _audioInterfaceProvider;
    private readonly IMonitorVolumeView _view;

    public MonitorVolumeCommand(IAudioInterfaceProvider audioInterfaceProvider, IMonitorVolumeView view)
    {
        _audioInterfaceProvider = audioInterfaceProvider;
        _view = view;
    }

    public string Name => "Monitor              : マイクの入力をモニターする。データの保存は行わない。";

    public Task ExecuteAsync()
    {
        using var microphones = _audioInterfaceProvider.Resolve();

        microphones.ActivateMicrophones();
        try
        {
            _view.NotifyDetailMessage();
            _view.StartNotifyMasterPeakValue(microphones);

            _view.WaitToBeStopped();

            _view.StopNotifyMasterPeakValue();

            return Task.CompletedTask;
        }
        finally
        {
            microphones.DeactivateMicrophones();
        }
    }
}