using MicrophoneLevelLogger;
using MicrophoneLevelLogger.Command;
using MicrophoneLevelLogger.Domain;
using MicrophoneLevelLogger.View;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();

        services.AddTransient<IMicrophonesProvider, MicrophonesProvider>();
        services.AddTransient<ICommandInvoker, CommandInvoker>();
        services.AddTransient<CalibrateCommand>();
        services.AddTransient<RecordCommand>();
        services.AddTransient<SetMaxInputLevelCommand>();
        services.AddTransient<ICommandInvokerView, CommandInvokerView>();
        services.AddTransient<ICalibrateView, CalibrateView>();
        services.AddTransient<IRecordView, RecordView>();
    })
    .ConfigureLogging((context, builder) =>
    {
        builder.ClearProviders();
    })
    .Build();

await host.RunAsync();