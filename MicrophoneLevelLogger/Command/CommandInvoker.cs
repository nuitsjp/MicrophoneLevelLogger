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
    private readonly MeasureInputLevelCommand _measureInputLevelCommand;
    private readonly ShowInputLevelCommand _showInputLevelCommand;
    private readonly DeleteInputLevelsCommand _deleteInputLevelsCommand;
    private readonly RecordingSettingsCommand _recordingSettingsCommand;
    private readonly ExitCommand _exitCommand = new();

    public CommandInvoker(
        IAudioInterfaceProvider audioInterfaceProvider,
        ICommandInvokerView view, 
        CalibrateCommand calibrateCommand, 
        RecordCommand recordCommand, 
        SetMaxInputLevelCommand setMaxInputLevelCommand, 
        MonitorVolumeCommand monitorVolumeCommand, 
        DeleteRecordCommand deleteRecordCommand, 
        MeasureInputLevelCommand measureInputLevelCommand, 
        ShowInputLevelCommand showInputLevelCommand, 
        DeleteInputLevelsCommand deleteInputLevelsCommand, 
        RecordingSettingsCommand recordingSettingsCommand)
    {
        _audioInterfaceProvider = audioInterfaceProvider;
        _view = view;
        _calibrateCommand = calibrateCommand;
        _recordCommand = recordCommand;
        _setMaxInputLevelCommand = setMaxInputLevelCommand;
        _monitorVolumeCommand = monitorVolumeCommand;
        _deleteRecordCommand = deleteRecordCommand;
        _measureInputLevelCommand = measureInputLevelCommand;
        _showInputLevelCommand = showInputLevelCommand;
        _deleteInputLevelsCommand = deleteInputLevelsCommand;
        _recordingSettingsCommand = recordingSettingsCommand;
    }

    public async Task InvokeAsync()
    {
        var microphones = _audioInterfaceProvider.Resolve();
        _view.NotifyMicrophonesInformation(microphones);

        while (true)
        {
            var commands = new ICommand[]
            {
                _monitorVolumeCommand,
                _setMaxInputLevelCommand,
                _measureInputLevelCommand,
                _showInputLevelCommand,
                _calibrateCommand,
                _recordCommand,
                _recordingSettingsCommand,
                _deleteInputLevelsCommand,
                _deleteRecordCommand,
                _exitCommand
            };
            var selected = _view.SelectCommand(commands.Select(x => x.Name));
            if (selected == _exitCommand.Name)
            {
                break;
            }

            try
            {
                var command = commands.Single(x => x.Name == selected);
                await command.ExecuteAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    private class ExitCommand : ICommand
    {
        public string Name => "Exit                 : 終了する。";

        public Task ExecuteAsync()
        {
            throw new NotImplementedException();
        }
    }
}