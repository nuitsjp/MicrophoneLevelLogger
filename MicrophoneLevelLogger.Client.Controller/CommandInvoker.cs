using MicrophoneLevelLogger.Client.Controller.CalibrateInput;
using MicrophoneLevelLogger.Client.Controller.CalibrateOutput;
using MicrophoneLevelLogger.Client.Controller.DeleteCalibrates;
using MicrophoneLevelLogger.Client.Controller.DeleteRecord;
using MicrophoneLevelLogger.Client.Controller.DisableMicrophone;
using MicrophoneLevelLogger.Client.Controller.DisplayAudioInterface;
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

namespace MicrophoneLevelLogger.Client.Controller;

/// <summary>
/// MicrophoneLevelLogger全体をコントロールし実行する。
/// </summary>
public class CommandInvoker : ICommandInvoker
{
    /// <summary>
    /// すべてのコントローラーを内包する復号コントローラー
    /// </summary>
    private readonly CompositeController _controller;

    public CommandInvoker(
        IAudioInterfaceProvider audioInterfaceProvider,
        IMicrophoneView microphoneView, 
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
        DisplayAudioInterfaceController displayAudioInterfaceController,
        ICompositeControllerView compositeControllerView)
    {
        // メニューをくみ上げる。
        _controller = new RootController(microphoneView, compositeControllerView, audioInterfaceProvider)
            .AddController(monitorVolumeController)
            .AddController(recordController)
            .AddController(displayRecordsController)
            .AddController(new BorderController())
            .AddController(displayAudioInterfaceController)
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

    /// <summary>
    /// 実行する。
    /// </summary>
    /// <returns></returns>
    public async Task InvokeAsync()
    {
        await _controller.ExecuteAsync();
    }

    /// <summary>
    /// 複合コントローラーを継承したルート専用のコントローラー
    /// </summary>
    private class RootController : CompositeController
    {
        /// <summary>
        /// ビュー
        /// </summary>
        private readonly IMicrophoneView _view;
        /// <summary>
        /// IAudioInterfaceプロバイダー
        /// </summary>
        private readonly IAudioInterfaceProvider _provider;

        public RootController(
            IMicrophoneView microphoneView,
            ICompositeControllerView compositeControllerView, 
            IAudioInterfaceProvider provider) : base(compositeControllerView)
        {
            _view = microphoneView;
            _provider = provider;
        }

        public override async Task ExecuteAsync()
        {
            // オーディオインターフェースの状態を表示する。
            await _view.NotifyAudioInterfaceAsync(_provider.Resolve());

            // 複合コントローラーを実行し、メニュー表示や選択されたコマンドを実行する。
            await base.ExecuteAsync();
        }
    }
}