using MicrophoneLevelLogger.Domain;
using MicrophoneLevelLogger.View;

namespace MicrophoneLevelLogger.Command;

public class CommandInvoker : ICommandInvoker
{
    private readonly IMicrophonesProvider _microphonesProvider;
    private readonly ICommandInvokerView _view;
    private readonly CalibrateCommand _calibrateCommand;
    private readonly RecordCommand _recordCommand;
    private readonly MonitorVolumeCommand _monitorVolumeCommand;
    private readonly SetMaxInputLevelCommand _setMaxInputLevelCommand;

    public CommandInvoker(
        IMicrophonesProvider microphonesProvider,
        ICommandInvokerView view, 
        CalibrateCommand calibrateCommand, 
        RecordCommand recordCommand, 
        SetMaxInputLevelCommand setMaxInputLevelCommand, 
        MonitorVolumeCommand monitorVolumeCommand)
    {
        _microphonesProvider = microphonesProvider;
        _view = view;
        _calibrateCommand = calibrateCommand;
        _recordCommand = recordCommand;
        _setMaxInputLevelCommand = setMaxInputLevelCommand;
        _monitorVolumeCommand = monitorVolumeCommand;
    }

    public async Task InvokeAsync()
    {
        var microphones = _microphonesProvider.Resolve();
        while (true)
        {
            _view.NotifyMicrophonesInformation(microphones);

            var commands = new ICommand[]
            {
                _monitorVolumeCommand,
                _calibrateCommand,
                _recordCommand,
                _setMaxInputLevelCommand
            };
            var selected = _view.SelectCommand(commands.Select(x => x.Name));
            var command = commands.Single(x => x.Name == selected);
            await command.ExecuteAsync();
        }
    }
}