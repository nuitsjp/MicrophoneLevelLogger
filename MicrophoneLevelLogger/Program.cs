﻿using MicrophoneLevelLogger;
using MicrophoneLevelLogger.Command;
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
using MicrophoneLevelLogger.View;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = Host.CreateDefaultBuilder((string[]) args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();

        services.AddTransient<IAudioInterfaceProvider, AudioInterfaceProvider>();

        services.AddTransient<ICommandInvoker, CommandInvoker>();
        services.AddTransient<ICommandInvokerView, CommandInvokerView>();

        services.AddTransient<CalibrateOutputCommand>();
        services.AddTransient<ICalibrateOutputView, CalibrateOutputView>();

        services.AddTransient<CalibrateInputCommand>();
        services.AddTransient<ICalibrateInputView, CalibrateInputView>();

        services.AddTransient<DisplayCalibratesCommand>();
        services.AddTransient<IDisplayCalibratesView, DisplayCalibratesView>();

        services.AddTransient<DeleteCalibratesCommand>();
        services.AddTransient<IDeleteCalibrateView, DeleteCalibrateView>();

        services.AddTransient<RecordCommand>();
        services.AddTransient<IRecordView, RecordView>();

        services.AddTransient<SetMaxInputLevelCommand>();

        services.AddTransient<MonitorVolumeCommand>();
        services.AddTransient<IMonitorVolumeView, MonitorVolumeView>();

        services.AddTransient<DeleteRecordCommand>();
        services.AddTransient<IDeleteRecordView, DeleteRecordView>();

        services.AddTransient<DisplayMeasurementsCommand>();
        services.AddTransient<IDisplayMeasurementsView, DisplayMeasurementsView>();

        services.AddTransient<DeleteInputLevelsCommand>();
        services.AddTransient<IDeleteInputLevelsView, DeleteInputLevelsView>();

        services.AddTransient<RecordingSettingsCommand>();
        services.AddTransient<IRecordingSettingsView, RecordingSettingsView>();

        services.AddTransient<MeasureCommand>();
        services.AddTransient<IMeasureView, MeasureView>();

        services.AddTransient<SetInputLevelCommand>();
        services.AddTransient<ISetInputLevelView, SetInputLevelView>();

        services.AddTransient<DisplayMicrophonesCommand>();
        services.AddTransient<IMicrophoneView, MicrophoneView>();

        services.AddTransient<IRecorderProvider, RecorderProvider>();
        services.AddTransient<IMediaPlayerProvider, MediaPlayerProvider>();
        services.AddTransient<IMediaPlayer, MediaPlayer>();
    })
    .ConfigureLogging((_, builder) =>
    {
        builder.ClearProviders();
    })
    .Build();

await host.RunAsync();