using MicrophoneLevelLogger.Client.Controller.CalibrateInput;
using MicrophoneLevelLogger.Client.Controller.CalibrateOutput;
using MicrophoneLevelLogger.Client.Controller.DeleteCalibrates;
using MicrophoneLevelLogger.Client.Controller.DeleteInputLevels;
using MicrophoneLevelLogger.Client.Controller.DeleteRecord;
using MicrophoneLevelLogger.Client.Controller.DisplayCalibrates;
using MicrophoneLevelLogger.Client.Controller.DisplayMeasurements;
using MicrophoneLevelLogger.Client.Controller.DisplayMicrophones;
using MicrophoneLevelLogger.Client.Controller.Measure;
using MicrophoneLevelLogger.Client.Controller.MonitorVolume;
using MicrophoneLevelLogger.Client.Controller.Record;
using MicrophoneLevelLogger.Client.Controller.RecordingSettings;
using MicrophoneLevelLogger.Client.Controller.SetInputLevel;
using MicrophoneLevelLogger.Client.Controller.SetMaxInputLevel;

namespace MicrophoneLevelLogger.Client.Controller;

public class CommandInvoker : ICommandInvoker
{
    private readonly IAudioInterfaceProvider _audioInterfaceProvider;
    private readonly ICommandInvokerView _view;
    private readonly CalibrateOutputController _calibrateOutputController;
    private readonly CalibrateInputController _calibrateInputController;
    private readonly RecordController _recordController;
    private readonly MonitorVolumeController _monitorVolumeController;
    private readonly SetMaxInputLevelController _setMaxInputLevelController;
    private readonly DeleteRecordController _deleteRecordController;
    private readonly DisplayMeasurementsController _displayMeasurementsController;
    private readonly DeleteInputLevelsController _deleteInputLevelsController;
    private readonly RecordingSettingsController _recordingSettingsController;
    private readonly MeasureController _measureController;
    private readonly DeleteCalibratesController _deleteCalibratesController;
    private readonly DisplayCalibratesController _displayCalibratesController;
    private readonly SetInputLevelController _setInputLevelController;
    private readonly DisplayMicrophonesController _displayMicrophonesController;
    private readonly ExitController _exitController = new();
    private readonly BorderController _borderController = new();

    public CommandInvoker(
        IAudioInterfaceProvider audioInterfaceProvider,
        ICommandInvokerView view, 
        CalibrateInputController calibrateInputController, 
        RecordController recordController, 
        SetMaxInputLevelController setMaxInputLevelController, 
        MonitorVolumeController monitorVolumeController, 
        DeleteRecordController deleteRecordController, 
        DisplayMeasurementsController displayMeasurementsController, 
        DeleteInputLevelsController deleteInputLevelsController, 
        RecordingSettingsController recordingSettingsController, 
        MeasureController measureController, 
        DeleteCalibratesController deleteCalibratesController, 
        DisplayCalibratesController displayCalibratesController, 
        CalibrateOutputController calibrateOutputController, 
        SetInputLevelController setInputLevelController, 
        DisplayMicrophonesController displayMicrophonesController)
    {
        _audioInterfaceProvider = audioInterfaceProvider;
        _view = view;
        _calibrateInputController = calibrateInputController;
        _recordController = recordController;
        _setMaxInputLevelController = setMaxInputLevelController;
        _monitorVolumeController = monitorVolumeController;
        _deleteRecordController = deleteRecordController;
        _displayMeasurementsController = displayMeasurementsController;
        _deleteInputLevelsController = deleteInputLevelsController;
        _recordingSettingsController = recordingSettingsController;
        _measureController = measureController;
        _deleteCalibratesController = deleteCalibratesController;
        _displayCalibratesController = displayCalibratesController;
        _calibrateOutputController = calibrateOutputController;
        _setInputLevelController = setInputLevelController;
        _displayMicrophonesController = displayMicrophonesController;
    }

    public async Task InvokeAsync()
    {
        while (true)
        {
            var microphones = _audioInterfaceProvider.Resolve();
            _view.NotifyMicrophonesInformation(microphones);

            var commands = new IController[]
            {
                _displayMicrophonesController,
                _monitorVolumeController,
                _measureController,
                _displayMeasurementsController,
                _setMaxInputLevelController,
                _calibrateInputController,
                _calibrateOutputController,
                _displayCalibratesController,
                _setInputLevelController,
                _recordController,
                _recordingSettingsController,
                _deleteInputLevelsController,
                _deleteCalibratesController,
                _deleteRecordController,
                _borderController,
                _exitController
            };
            var selected = _view.SelectCommand(commands.Select(x => x.Name));
            if (selected == _exitController.Name)
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

    private class ExitController : IController
    {
        public string Name => "Exit                 : 終了する。";

        public Task ExecuteAsync()
        {
            throw new NotImplementedException();
        }
    }

    private class BorderController : IController
    {
        public string Name => "-----------------------------------------------------------";
        public Task ExecuteAsync() => Task.CompletedTask;
    }
}