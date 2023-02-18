using MicrophoneLevelLogger.Client.Controller.Record;
using Microsoft.AspNetCore.Mvc;

namespace MicrophoneLevelLogger.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class RecorderController : ControllerBase
{
    private static CancellationTokenSource _cancellationTokenSource = new();
    private readonly IAudioInterfaceProvider _audioInterfaceProvider;
    private readonly IRecorderProvider _recorderProvider;
    private readonly IRecordView _view;

    public RecorderController(
        IAudioInterfaceProvider audioInterfaceProvider, 
        IRecorderProvider recorderProvider, 
        IRecordView view)
    {
        _audioInterfaceProvider = audioInterfaceProvider;
        _recorderProvider = recorderProvider;
        _view = view;
    }

    [HttpGet("Recode")]
    public Task RecodeAsync() => RecodeAsync(string.Empty);

    [HttpGet("Recode/{recordName}")]
    public async Task RecodeAsync(string recordName)
    {
        Console.WriteLine($"Recorder#Record name:{recordName}");
        var audioInterface = _audioInterfaceProvider.Resolve();
        var logger = _recorderProvider.ResolveLocal(audioInterface, recordName);
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