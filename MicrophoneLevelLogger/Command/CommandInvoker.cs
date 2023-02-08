using MicrophoneLevelLogger.Domain;

namespace MicrophoneLevelLogger.Command;

public class CommandInvoker : ICommandInvoker
{
    private readonly IAudioInterfaceProvider _audioInterfaceProvider;
    private readonly ICommandInvokerView _view;
    private readonly CalibrateCommand _calibrateCommand;
    private readonly RecordCommand _recordCommand;
    private readonly MonitorVolumeCommand _monitorVolumeCommand;
    private readonly SetMaxInputLevelCommand _setMaxInputLevelCommand;
    private readonly DeleteRecordCommand _deleteRecordCommand;
    private readonly ExitCommand _exitCommand = new();

    public CommandInvoker(
        IAudioInterfaceProvider audioInterfaceProvider,
        ICommandInvokerView view, 
        CalibrateCommand calibrateCommand, 
        RecordCommand recordCommand, 
        SetMaxInputLevelCommand setMaxInputLevelCommand, 
        MonitorVolumeCommand monitorVolumeCommand, 
        DeleteRecordCommand deleteRecordCommand)
    {
        _audioInterfaceProvider = audioInterfaceProvider;
        _view = view;
        _calibrateCommand = calibrateCommand;
        _recordCommand = recordCommand;
        _setMaxInputLevelCommand = setMaxInputLevelCommand;
        _monitorVolumeCommand = monitorVolumeCommand;
        _deleteRecordCommand = deleteRecordCommand;
    }

    public async Task InvokeAsync()
    {
        while (true)
        {
            var microphones = _audioInterfaceProvider.Resolve();
            _view.NotifyMicrophonesInformation(microphones);

            var commands = new ICommand[]
            {
                _monitorVolumeCommand,
                _calibrateCommand,
                _recordCommand,
                _setMaxInputLevelCommand,
                _deleteRecordCommand,
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
        public string Name => "Exit        : 終了する。";

        public Task ExecuteAsync()
        {
            throw new NotImplementedException();
        }
    }
}