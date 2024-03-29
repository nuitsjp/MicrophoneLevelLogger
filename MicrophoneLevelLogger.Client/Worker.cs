﻿using MicrophoneLevelLogger.Client.Controller;
using Microsoft.Extensions.Hosting;

namespace MicrophoneLevelLogger.Client;

/// <summary>
/// Generic Host上で、コンソールアプリケーションとして動作させるためのワーカー
/// </summary>
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