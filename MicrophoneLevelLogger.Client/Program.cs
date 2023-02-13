using MicrophoneLevelLogger;
using MicrophoneLevelLogger.Client;
using MicrophoneLevelLogger.Client.Controller;
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
using MicrophoneLevelLogger.Client.View;
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
        services.AddTransient<IRecorderProvider, RecorderProvider>();
        services.AddTransient<IMediaPlayerProvider, MediaPlayerProvider>();
        services.AddTransient<IMediaPlayer, MediaPlayer>();

        /////////////////////////////////////////////////////////////////////////
        // Domain & View
        /////////////////////////////////////////////////////////////////////////
        services.AddTransient<ICommandInvoker, CommandInvoker>();
        services.AddTransient<ICommandInvokerView, CommandInvokerView>();

        services.AddTransient<CalibrateInputCommand>();
        services.AddTransient<ICalibrateInputView, CalibrateInputView>();

        services.AddTransient<CalibrateOutputCommand>();
        services.AddTransient<ICalibrateOutputView, CalibrateOutputView>();

        services.AddTransient<DeleteCalibratesCommand>();
        services.AddTransient<IDeleteCalibrateView, DeleteCalibrateView>();

        services.AddTransient<DeleteInputLevelsCommand>();
        services.AddTransient<IDeleteInputLevelsView, DeleteInputLevelsView>();

        services.AddTransient<DeleteRecordCommand>();
        services.AddTransient<IDeleteRecordView, DeleteRecordView>();

        services.AddTransient<DisplayCalibratesCommand>();
        services.AddTransient<IDisplayCalibratesView, DisplayCalibratesView>();

        services.AddTransient<DisplayMeasurementsCommand>();
        services.AddTransient<IDisplayMeasurementsView, DisplayMeasurementsView>();

        services.AddTransient<DisplayMicrophonesCommand>();
        services.AddTransient<IMicrophoneView, MicrophoneView>();

        services.AddTransient<MeasureCommand>();
        services.AddTransient<IMeasureView, MeasureView>();

        services.AddTransient<MonitorVolumeCommand>();
        services.AddTransient<IMonitorVolumeView, MonitorVolumeView>();

        services.AddTransient<RecordCommand>();
        services.AddTransient<IRecordView, RecordView>();

        services.AddTransient<RecordingSettingsCommand>();
        services.AddTransient<IRecordingSettingsView, RecordingSettingsView>();

        services.AddTransient<SetInputLevelCommand>();
        services.AddTransient<ISetInputLevelView, SetInputLevelView>();

        services.AddTransient<SetMaxInputLevelCommand>();
    })
    .ConfigureLogging((_, builder) =>
    {
        builder.ClearProviders();
    })
    .Build();

await host.RunAsync();