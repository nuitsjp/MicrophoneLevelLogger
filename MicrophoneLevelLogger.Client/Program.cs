using MicrophoneLevelLogger;
using MicrophoneLevelLogger.Client;
using MicrophoneLevelLogger.Client.Controller;
using MicrophoneLevelLogger.Client.Controller.CalibrateInput;
using MicrophoneLevelLogger.Client.Controller.CalibrateOutput;
using MicrophoneLevelLogger.Client.Controller.DeleteCalibrates;
using MicrophoneLevelLogger.Client.Controller.DeleteRecord;
using MicrophoneLevelLogger.Client.Controller.DisableMicrophone;
using MicrophoneLevelLogger.Client.Controller.DisplayCalibrates;
using MicrophoneLevelLogger.Client.Controller.DisplayRecords;
using MicrophoneLevelLogger.Client.Controller.ExcludeMicrophone;
using MicrophoneLevelLogger.Client.Controller.MonitorVolume;
using MicrophoneLevelLogger.Client.Controller.Record;
using MicrophoneLevelLogger.Client.Controller.RecordingSettings;
using MicrophoneLevelLogger.Client.Controller.RemoveAlias;
using MicrophoneLevelLogger.Client.Controller.SetAlias;
using MicrophoneLevelLogger.Client.Controller.SetInputLevel;
using MicrophoneLevelLogger.Client.Controller.SetMaxInputLevel;
using MicrophoneLevelLogger.Client.View;
using MicrophoneLevelLogger.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = Host.CreateDefaultBuilder((string[])args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();

        /////////////////////////////////////////////////////////////////////////
        // Domain
        /////////////////////////////////////////////////////////////////////////
        services.AddTransient<IAudioInterfaceProvider, AudioInterfaceProvider>();
        services.AddTransient<IMediaPlayerProvider, MediaPlayerProvider>();
        services.AddTransient<IMediaPlayer, MediaPlayer>();
        services.AddTransient<IRecorderProvider, RecorderProvider>();

        /////////////////////////////////////////////////////////////////////////
        // Repository
        /////////////////////////////////////////////////////////////////////////
        services.AddTransient<IAudioInterfaceCalibrationValuesRepository, AudioInterfaceCalibrationValuesRepository>();
        services.AddTransient<ISettingsRepository, SettingsRepository>();
        services.AddTransient<IRecordSummaryRepository, RecordSummaryRepository>();

        /////////////////////////////////////////////////////////////////////////
        // Controller & View
        /////////////////////////////////////////////////////////////////////////
        services.AddTransient<ICommandInvoker, CommandInvoker>();
        services.AddTransient<ICommandInvokerView, CommandInvokerView>();
        services.AddTransient<ICompositeControllerView, CompositeControllerView>();

        services.AddTransient<IMicrophoneView, MicrophoneView>();

        services.AddTransient<CalibrateInputController>();
        services.AddTransient<ICalibrateInputView, CalibrateInputView>();

        services.AddTransient<CalibrateOutputController>();
        services.AddTransient<ICalibrateOutputView, CalibrateOutputView>();

        services.AddTransient<DeleteCalibratesController>();
        services.AddTransient<IDeleteCalibrateView, DeleteCalibrateView>();

        services.AddTransient<DeleteRecordController>();
        services.AddTransient<IDeleteRecordView, DeleteRecordView>();

        services.AddTransient<DisplayCalibratesController>();
        services.AddTransient<IDisplayCalibratesView, DisplayCalibratesView>();


        services.AddTransient<MonitorVolumeController>();
        services.AddTransient<IMonitorVolumeView, MonitorVolumeView>();

        services.AddTransient<RecordController>();
        services.AddTransient<IRecordView, RecordView>();

        services.AddTransient<DisplayRecordsController>();
        services.AddTransient<IDisplayRecordsView, DisplayRecordsView>();

        services.AddTransient<RecordingSettingsController>();
        services.AddTransient<IRecordingSettingsView, RecordingSettingsView>();

        services.AddTransient<SetInputLevelController>();
        services.AddTransient<ISetInputLevelView, SetInputLevelView>();

        services.AddTransient<SetAliasController>();
        services.AddTransient<ISetAliasView, SetAliasView>();

        services.AddTransient<RemoveAliasController>();
        services.AddTransient<IRemoveAliasView, RemoveAliasView>();

        services.AddTransient<DisableMicrophoneController>();
        services.AddTransient<IDisableMicrophoneView, DisableMicrophoneView>();

        services.AddTransient<SetMaxInputLevelController>();
    })
    .ConfigureLogging((_, builder) =>
    {
        builder.ClearProviders();
    })
    .Build();

await host.RunAsync();