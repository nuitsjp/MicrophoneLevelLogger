using System.Drawing;
using MicrophoneLevelLogger.Client.Controller.CalibrateInput;
using MicrophoneLevelLogger.Client.Controller.CalibrateOutput;
using MicrophoneLevelLogger.Client.Controller.DeleteCalibrates;
using MicrophoneLevelLogger.Client.Controller.DeleteRecord;
using MicrophoneLevelLogger.Client.Controller.DisableMicrophone;
using MicrophoneLevelLogger.Client.Controller.DisplayCalibrates;
using MicrophoneLevelLogger.Client.Controller.DisplayRecords;
using MicrophoneLevelLogger.Client.Controller.EnableMicrophone;
using MicrophoneLevelLogger.Client.Controller.MonitorVolume;
using MicrophoneLevelLogger.Client.Controller.Record;
using MicrophoneLevelLogger.Client.Controller.RecordingSettings;
using MicrophoneLevelLogger.Client.Controller.RemoveAlias;
using MicrophoneLevelLogger.Client.Controller.SelectSpeaker;
using MicrophoneLevelLogger.Client.Controller.SetAlias;
using MicrophoneLevelLogger.Client.Controller.SetInputLevel;
using MicrophoneLevelLogger.Client.Controller.SetMaxInputLevel;
using static System.ComponentModel.Design.ObjectSelectorEditor;

namespace MicrophoneLevelLogger.Client.Controller;

public class CommandInvoker : ICommandInvoker
{
    private readonly IAudioInterfaceProvider _audioInterfaceProvider;
    private readonly ICommandInvokerView _view;

    private readonly CompositeController _compositeController;

    public CommandInvoker(
        IAudioInterfaceProvider audioInterfaceProvider,
        ICommandInvokerView view, 
        CalibrateInputController calibrateInputController, 
        RecordController recordController, 
        SetMaxInputLevelController setMaxInputLevelController, 
        MonitorVolumeController monitorVolumeController, 
        DeleteRecordController deleteRecordController, 
        RecordingSettingsController recordingSettingsController, 
        DeleteCalibratesController deleteCalibratesController, 
        DisplayCalibratesController displayCalibratesController, 
        CalibrateOutputController calibrateOutputController, 
        SetInputLevelController setInputLevelController, 
        DisplayRecordsController displayRecordsController, 
        SetAliasController setAliasController,
        RemoveAliasController removeAliasController,
        DisableMicrophoneController disableMicrophoneController,
        EnableMicrophoneController enableMicrophoneController,
        SelectSpeakerController selectSpeakerController,
        ICompositeControllerView compositeControllerView)
    {
        _audioInterfaceProvider = audioInterfaceProvider;
        _view = view;
        _compositeController = new CompositeController(compositeControllerView)
            .AddController(monitorVolumeController)
            .AddController(recordController)
            .AddController(displayRecordsController)
            .AddController(new BorderController())
            .AddController(new RedisplayMicrophoneController())
            .AddController(
                new CompositeController(
                        "Settings",
                        "各種設定を変更します。",
                        compositeControllerView)
                    .AddController(recordingSettingsController)
                    .AddController(
                        new CompositeController(
                                "Alias",
                                "マイクの別名を設定します。",
                                compositeControllerView)
                            .AddController(setAliasController)
                            .AddController(removeAliasController))
                    .AddController(disableMicrophoneController)
                    .AddController(enableMicrophoneController)
                    .AddController(selectSpeakerController))
            .AddController(
                new CompositeController(
                        "Calibrate",
                        "マイク・スピーカーを調整します。",
                        compositeControllerView)
                    .AddController(setMaxInputLevelController)
                    .AddController(setInputLevelController)
                    .AddController(calibrateInputController)
                    .AddController(displayCalibratesController)
                    .AddController(calibrateOutputController))
            .AddController(
                new CompositeController(
                        "Delete",
                        "各種情報を削除します。",
                        compositeControllerView)
                    .AddController(deleteCalibratesController)
                    .AddController(deleteRecordController));
    }

    public async Task InvokeAsync()
    {
        var microphones = _audioInterfaceProvider.Resolve();
        _view.NotifyMicrophonesInformation(microphones);

        await _compositeController.ExecuteAsync();
    }

    private class RedisplayMicrophoneController : IController
    {
        public string Name => "Display microphone";
        public string Description => "マイクの情報を再表示します。";

        public Task ExecuteAsync()
        {
            return Task.CompletedTask;
        }
    }
}