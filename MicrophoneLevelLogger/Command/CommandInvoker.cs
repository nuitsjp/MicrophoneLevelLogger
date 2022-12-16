using MicrophoneLevelLogger.View;

namespace MicrophoneLevelLogger.Command;

public class CommandInvoker : ICommandInvoker
{
    private readonly ICommandInvokerView _view;
    private readonly CalibrateCommand _calibrateCommand;
    private readonly RecordCommand _recordCommand;
    private readonly SetMaxInputLevelCommand _setMaxInputLevelCommand;

    public CommandInvoker(
        ICommandInvokerView view, 
        CalibrateCommand calibrateCommand, 
        RecordCommand recordCommand, 
        SetMaxInputLevelCommand setMaxInputLevelCommand)
    {
        _view = view;
        _calibrateCommand = calibrateCommand;
        _recordCommand = recordCommand;
        _setMaxInputLevelCommand = setMaxInputLevelCommand;
    }

    public async Task InvokeAsync()
    {
        while (true)
        {
            var commands = new ICommand[]
            {
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