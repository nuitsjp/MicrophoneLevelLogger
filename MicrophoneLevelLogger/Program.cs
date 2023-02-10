using MicrophoneLevelLogger;
using MicrophoneLevelLogger.Command;
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

        services.AddTransient<CalibrateCommand>();
        services.AddTransient<ICalibrateView, CalibrateView>();

        services.AddTransient<RecordCommand>();
        services.AddTransient<IRecordView, RecordView>();

        services.AddTransient<SetMaxInputLevelCommand>();

        services.AddTransient<MonitorVolumeCommand>();
        services.AddTransient<IMonitorVolumeView, MonitorVolumeView>();

        services.AddTransient<DeleteRecordCommand>();

        services.AddTransient<DisplayMeasurementsCommand>();
        services.AddTransient<IDisplayMeasurementsView, DisplayMeasurementsView>();

        services.AddTransient<DeleteInputLevelsCommand>();
        services.AddTransient<IRemoveInputLevelsView, RemoveInputLevelsView>();

        services.AddTransient<RecordingSettingsCommand>();
        services.AddTransient<IRecordingSettingsView, RecordingSettingsView>();

        services.AddTransient<MeasureCommand>();
        services.AddTransient<IMeasureView, MeasureView>();

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