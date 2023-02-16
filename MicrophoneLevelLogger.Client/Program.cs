﻿using MicrophoneLevelLogger;
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
        services.AddTransient<IRecorderProvider, RecorderProvider>();
        services.AddTransient<IMediaPlayerProvider, MediaPlayerProvider>();
        services.AddTransient<IMediaPlayer, MediaPlayer>();
        services.AddTransient<IAudioInterfaceLoggerProvider, AudioInterfaceLoggerProvider>();

        /////////////////////////////////////////////////////////////////////////
        // Repository
        /////////////////////////////////////////////////////////////////////////
        services.AddTransient<IAudioInterfaceCalibrationValuesRepository, AudioInterfaceCalibrationValuesRepository>();
        services.AddTransient<IAudioInterfaceInputLevelsRepository, AudioInterfaceInputLevelsRepository>();
        services.AddTransient<IRecordingSettingsRepository, RecordingSettingsRepository>();

        /////////////////////////////////////////////////////////////////////////
        // Controller & View
        /////////////////////////////////////////////////////////////////////////
        services.AddTransient<ICommandInvoker, CommandInvoker>();
        services.AddTransient<ICommandInvokerView, CommandInvokerView>();

        services.AddTransient<CalibrateInputController>();
        services.AddTransient<ICalibrateInputView, CalibrateInputView>();

        services.AddTransient<CalibrateOutputController>();
        services.AddTransient<ICalibrateOutputView, CalibrateOutputView>();

        services.AddTransient<DeleteCalibratesController>();
        services.AddTransient<IDeleteCalibrateView, DeleteCalibrateView>();

        services.AddTransient<DeleteInputLevelsController>();
        services.AddTransient<IDeleteInputLevelsView, DeleteInputLevelsView>();

        services.AddTransient<DeleteRecordController>();
        services.AddTransient<IDeleteRecordView, DeleteRecordView>();

        services.AddTransient<DisplayCalibratesController>();
        services.AddTransient<IDisplayCalibratesView, DisplayCalibratesView>();

        services.AddTransient<DisplayMeasurementsController>();
        services.AddTransient<IDisplayMeasurementsView, DisplayMeasurementsView>();

        services.AddTransient<DisplayMicrophonesController>();
        services.AddTransient<IMicrophoneView, MicrophoneView>();

        services.AddTransient<MeasureController>();
        services.AddTransient<IMeasureView, MeasureView>();

        services.AddTransient<MonitorVolumeController>();
        services.AddTransient<IMonitorVolumeView, MonitorVolumeView>();

        services.AddTransient<RecordController>();
        services.AddTransient<IRecordView, RecordView>();

        services.AddTransient<RecordingSettingsController>();
        services.AddTransient<IRecordingSettingsView, RecordingSettingsView>();

        services.AddTransient<SetInputLevelController>();
        services.AddTransient<ISetInputLevelView, SetInputLevelView>();

        services.AddTransient<SetMaxInputLevelController>();
    })
    .ConfigureLogging((_, builder) =>
    {
        builder.ClearProviders();
    })
    .Build();

await host.RunAsync();