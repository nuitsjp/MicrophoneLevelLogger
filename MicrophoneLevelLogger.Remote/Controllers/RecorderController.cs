using MicrophoneLevelLogger.Domain;
using Microsoft.AspNetCore.Mvc;

namespace MicrophoneLevelLogger.Remote.Controllers;

[ApiController]
[Route("[controller]")]
public class RecorderController : ControllerBase
{
    private readonly IRecorder _recorder;

    public RecorderController(IRecorder recorder)
    {
        _recorder = recorder;
    }

    [HttpGet("Recode")]
    public Task RecodeAsync()
    {
        Console.WriteLine("Recorder#Record");
        return _recorder.RecodeAsync();
    }

    [HttpGet("Stop")]
    public Task StopAsync()
    {
        Console.WriteLine("Recorder#Stop");
        return _recorder.StopAsync();
    }
}