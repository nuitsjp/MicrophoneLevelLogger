using MicrophoneLevelLogger.Domain;

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

    public string Name => "Monitor     : マイクの入力をモニターする。データの保存は行わない。";

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