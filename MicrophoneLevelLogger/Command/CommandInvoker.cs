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
    private readonly ExitCommand _exitCommand = new();

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
        while (true)
        {
            var microphones = _microphonesProvider.Resolve();
            _view.NotifyMicrophonesInformation(microphones);

            var commands = new ICommand[]
            {
                _monitorVolumeCommand,
                _calibrateCommand,
                _recordCommand,
                _setMaxInputLevelCommand,
                _exitCommand
            };
            var selected = _view.SelectCommand(commands.Select(x => x.Name));
            if (selected == _exitCommand.Name)
            {
                break;
            }

            var command = commands.Single(x => x.Name == selected);
            await command.ExecuteAsync();
        }
    }

    private class ExitCommand : ICommand
    {
        public string Name => "Exit";

        public Task ExecuteAsync()
        {
            throw new NotImplementedException();
        }
    }
}