using MicrophoneLevelLogger.Client.Controller.Record;
using Microsoft.AspNetCore.Mvc;

namespace MicrophoneLevelLogger.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class RecorderController : ControllerBase
{
    private static CancellationTokenSource _cancellationTokenSource = new();
    private readonly IAudioInterfaceProvider _audioInterfaceProvider;
    private readonly IAudioInterfaceLoggerProvider _audioInterfaceLoggerProvider;
    private readonly IRecordView _view;

    public RecorderController(
        IAudioInterfaceProvider audioInterfaceProvider, 
        IAudioInterfaceLoggerProvider audioInterfaceLoggerProvider, 
        IRecordView view)
    {
        _audioInterfaceProvider = audioInterfaceProvider;
        _audioInterfaceLoggerProvider = audioInterfaceLoggerProvider;
        _view = view;
    }

    [HttpGet("Recode")]
    public Task RecodeAsync() => RecodeAsync(string.Empty);

    [HttpGet("Recode/{recordName}")]
    public async Task RecodeAsync(string recordName)
    {
        Console.WriteLine($"Recorder#Record name:{recordName}");
        var audioInterface = _audioInterfaceProvider.Resolve();
        var logger = _audioInterfaceLoggerProvider.ResolveLocal(audioInterface, recordName);
        _cancellationTokenSource = new();
        await logger.StartAsync(_cancellationTokenSource.Token);
        _view.StartNotify(logger, _cancellationTokenSource.Token);
    }

    [HttpGet("Stop")]
    public Task StopAsync()
    {
        Console.WriteLine("Recorder#Stop");
        _cancellationTokenSource.Cancel();
        return Task.CompletedTask;
    }
}