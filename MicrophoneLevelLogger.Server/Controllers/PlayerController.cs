using Microsoft.AspNetCore.Mvc;
// ReSharper disable LocalizableElement

namespace MicrophoneLevelLogger.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class PlayerController : ControllerBase
{
    private readonly IMediaPlayer _mediaPlayer;

    private static CancellationTokenSource _cancellationTokenSource = new();

    public PlayerController(IMediaPlayer mediaPlayer)
    {
        _mediaPlayer = mediaPlayer;
    }

    [HttpGet("Play")]
    public Task PlayAsync()
    {
        Console.WriteLine("Player#Play");
        _cancellationTokenSource = new();
        return _mediaPlayer.PlayLoopingAsync(_cancellationTokenSource.Token);
    }

    [HttpGet("Stop")]
    public Task StopAsync()
    {
        Console.WriteLine("Player#Stop");
        _cancellationTokenSource.Cancel();
        return Task.CompletedTask;
    }
}