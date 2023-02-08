using MicrophoneLevelLogger;
using MicrophoneLevelLogger.Command;
using MicrophoneLevelLogger.Domain;
using MicrophoneLevelLogger.View;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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

    })
    .ConfigureLogging((_, builder) =>
    {
        builder.ClearProviders();
    })
    .Build();

await host.RunAsync();