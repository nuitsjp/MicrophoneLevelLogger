using MicrophoneLevelLogger;
using MicrophoneLevelLogger.Client;
using MicrophoneLevelLogger.Client.Command;
using MicrophoneLevelLogger.Client.Command.CalibrateInput;
using MicrophoneLevelLogger.Client.Command.CalibrateOutput;
using MicrophoneLevelLogger.Client.Command.DeleteCalibrates;
using MicrophoneLevelLogger.Client.Command.DeleteInputLevels;
using MicrophoneLevelLogger.Client.Command.DeleteRecord;
using MicrophoneLevelLogger.Client.Command.DisplayCalibrates;
using MicrophoneLevelLogger.Client.Command.DisplayMeasurements;
using MicrophoneLevelLogger.Client.Command.DisplayMicrophones;
using MicrophoneLevelLogger.Client.Command.Measure;
using MicrophoneLevelLogger.Client.Command.MonitorVolume;
using MicrophoneLevelLogger.Client.Command.Record;
using MicrophoneLevelLogger.Client.Command.RecordingSettings;
using MicrophoneLevelLogger.Client.Command.SetInputLevel;
using MicrophoneLevelLogger.Client.Command.SetMaxInputLevel;
using MicrophoneLevelLogger.Client.View;
using MicrophoneLevelLogger.Domain;
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