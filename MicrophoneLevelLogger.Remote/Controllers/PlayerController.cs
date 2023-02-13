using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MicrophoneLevelLogger.Remote.Controllers;

[ApiController]
[Route("[controller]")]
public class PlayerController : ControllerBase
{
    private readonly IMediaPlayer _mediaPlayer;

    public PlayerController(IMediaPlayer mediaPlayer)
    {
        _mediaPlayer = mediaPlayer;
    }

    [HttpGet("Play")]
    public Task PlayAsync()
    {
        Console.WriteLine("Player#Play");
        return _mediaPlayer.PlayLoopingAsync();
    }

    [HttpGet("Stop")]
    public Task StopAsync()
    {
        Console.WriteLine("Player#Stop");
        return _mediaPlayer.StopAsync();
    }
}