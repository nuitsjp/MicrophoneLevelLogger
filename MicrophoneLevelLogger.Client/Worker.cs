using MicrophoneLevelLogger.Client.Command;
using Microsoft.Extensions.Hosting;

namespace MicrophoneLevelLogger.Client;

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
}