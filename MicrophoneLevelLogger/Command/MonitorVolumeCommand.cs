using MicrophoneLevelLogger.Domain;
using MicrophoneLevelLogger.View;

namespace MicrophoneLevelLogger.Command;

public class MonitorVolumeCommand : ICommand
{
    private readonly IMicrophonesProvider _microphonesProvider;
    private readonly IMonitorVolumeView _view;

    public MonitorVolumeCommand(IMicrophonesProvider microphonesProvider, IMonitorVolumeView view)
    {
        _microphonesProvider = microphonesProvider;
        _view = view;
    }

    public string Name => "Monitor microphone input volume";

    public Task ExecuteAsync()
    {
        using var microphones = _microphonesProvider.Resolve();

        microphones.Activate();
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
            microphones.Deactivate();
        }
    }
}