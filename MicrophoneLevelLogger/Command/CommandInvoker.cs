using MicrophoneLevelLogger.Command.CalibrateInput;
using MicrophoneLevelLogger.Command.DeleteInputLevels;
using MicrophoneLevelLogger.Command.DeleteRecord;
using MicrophoneLevelLogger.Command.DisplayCalibrates;
using MicrophoneLevelLogger.Command.DisplayMeasurements;
using MicrophoneLevelLogger.Command.DisplayMicrophones;
using MicrophoneLevelLogger.Command.Measure;
using MicrophoneLevelLogger.Command.MonitorVolume;
using MicrophoneLevelLogger.Command.Record;
using MicrophoneLevelLogger.Command.RecordingSettings;
using MicrophoneLevelLogger.Command.SetInputLevel;
using MicrophoneLevelLogger.Command.SetMaxInputLevel;
using MicrophoneLevelLogger.Domain;

namespace MicrophoneLevelLogger.Command;

public class CommandInvoker : ICommandInvoker
{
    private readonly IAudioInterfaceProvider _audioInterfaceProvider;
    private readonly ICommandInvokerView _view;
    private readonly CalibrateOutputCommand _calibrateOutputCommand;
    private readonly CalibrateInputCommand _calibrateInputCommand;
    private readonly RecordCommand _recordCommand;
    private readonly MonitorVolumeCommand _monitorVolumeCommand;
    private readonly SetMaxInputLevelCommand _setMaxInputLevelCommand;
    private readonly DeleteRecordCommand _deleteRecordCommand;
    private readonly DisplayMeasurementsCommand _displayMeasurementsCommand;
    private readonly DeleteInputLevelsCommand _deleteInputLevelsCommand;
    private readonly RecordingSettingsCommand _recordingSettingsCommand;
    private readonly MeasureCommand _measureCommand;
    private readonly DeleteCalibratesCommand _deleteCalibratesCommand;
    private readonly DisplayCalibratesCommand _displayCalibratesCommand;
    private readonly SetInputLevelCommand _setInputLevelCommand;
    private readonly DisplayMicrophonesCommand _displayMicrophonesCommand;
    private readonly ExitCommand _exitCommand = new();

    public CommandInvoker(
        IAudioInterfaceProvider audioInterfaceProvider,
        ICommandInvokerView view, 
        CalibrateInputCommand calibrateInputCommand, 
        RecordCommand recordCommand, 
        SetMaxInputLevelCommand setMaxInputLevelCommand, 
        MonitorVolumeCommand monitorVolumeCommand, 
        DeleteRecordCommand deleteRecordCommand, 
        DisplayMeasurementsCommand displayMeasurementsCommand, 
        DeleteInputLevelsCommand deleteInputLevelsCommand, 
        RecordingSettingsCommand recordingSettingsCommand, 
        MeasureCommand measureCommand, 
        DeleteCalibratesCommand deleteCalibratesCommand, 
        DisplayCalibratesCommand displayCalibratesCommand, 
        CalibrateOutputCommand calibrateOutputCommand, 
        SetInputLevelCommand setInputLevelCommand, 
        DisplayMicrophonesCommand displayMicrophonesCommand)
    {
        _audioInterfaceProvider = audioInterfaceProvider;
        _view = view;
        _calibrateInputCommand = calibrateInputCommand;
        _recordCommand = recordCommand;
        _setMaxInputLevelCommand = setMaxInputLevelCommand;
        _monitorVolumeCommand = monitorVolumeCommand;
        _deleteRecordCommand = deleteRecordCommand;
        _displayMeasurementsCommand = displayMeasurementsCommand;
        _deleteInputLevelsCommand = deleteInputLevelsCommand;
        _recordingSettingsCommand = recordingSettingsCommand;
        _measureCommand = measureCommand;
        _deleteCalibratesCommand = deleteCalibratesCommand;
        _displayCalibratesCommand = displayCalibratesCommand;
        _calibrateOutputCommand = calibrateOutputCommand;
        _setInputLevelCommand = setInputLevelCommand;
        _displayMicrophonesCommand = displayMicrophonesCommand;
    }

    public async Task InvokeAsync()
    {
        var microphones = _audioInterfaceProvider.Resolve();
        _view.NotifyMicrophonesInformation(microphones);

        while (true)
        {
            var commands = new ICommand[]
            {
                _displayMicrophonesCommand,
                _monitorVolumeCommand,
                _measureCommand,
                _displayMeasurementsCommand,
                _setMaxInputLevelCommand,
                _calibrateInputCommand,
                _calibrateOutputCommand,
                _displayCalibratesCommand,
                _setInputLevelCommand,
                _recordCommand,
                _recordingSettingsCommand,
                _deleteInputLevelsCommand,
                _deleteCalibratesCommand,
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