using MicrophoneLevelLogger.Command;
using MicrophoneLevelLogger.Domain;
using MicrophoneLevelLogger.View;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MicrophoneLevelLogger;

public class Worker : BackgroundService
{
    private readonly ICommandInvoker _commandInvoker;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;

    public Worker(
        ICommandInvoker commandInvoker, 
        IHostApplicationLifetime hostApplicationLifetime)
    {
        _commandInvoker = commandInvoker;
        _hostApplicationLifetime = hostApplicationLifetime;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _commandInvoker.InvokeAsync();

        _hostApplicationLifetime.StopApplication();
    }

    public static async Task RunAsync(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
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

                services.AddTransient<MeasureInputLevelCommand>();
                services.AddTransient<IMeasureInputLevelView, MeasureInputLevelView>();

                services.AddTransient<ShowInputLevelCommand>();
                services.AddTransient<IShowInputLevelView, ShowInputLevelView>();

                services.AddTransient<DeleteInputLevelsCommand>();
                services.AddTransient<IRemoveInputLevelsView, RemoveInputLevelsView>();

                services.AddTransient<RecordingSettingsCommand>();
                services.AddTransient<IRecordingSettingsView, RecordingSettingsView>();

                services.AddTransient<IRecorderProvider, RecorderProvider>();
                services.AddTransient<IMediaPlayerProvider, MediaPlayerProvider>();
            })
            .ConfigureLogging((_, builder) =>
            {
                builder.ClearProviders();
            })
            .Build();

        await host.RunAsync();
    }
}