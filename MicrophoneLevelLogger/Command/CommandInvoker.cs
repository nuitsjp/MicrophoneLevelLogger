using MicrophoneLevelLogger.View;

namespace MicrophoneLevelLogger.Command;

public class CommandInvoker : ICommandInvoker
{
    private readonly ICommandInvokerView _view;
    private readonly CalibrateCommand _calibrateCommand;
    private readonly RecordCommand _recordCommand;

    public CommandInvoker(
        ICommandInvokerView view, 
        CalibrateCommand calibrateCommand, 
        RecordCommand recordCommand)
    {
        _view = view;
        _calibrateCommand = calibrateCommand;
        _recordCommand = recordCommand;
    }

    public async Task InvokeAsync()
    {
        while (true)
        {
            var commands = new ICommand[]
            {
                _calibrateCommand,
                _recordCommand
            };
            var selected = _view.SelectCommand(commands.Select(x => x.Name));
            var command = commands.Single(x => x.Name == selected);
            await command.ExecuteAsync();
        }
    }
}