namespace MicrophoneLevelLogger.Client.Controller.MonitorVolume;

public class MonitorVolumeController : IController
{
    private readonly IAudioInterfaceProvider _audioInterfaceProvider;
    private readonly IAudioInterfaceLoggerProvider _audioInterfaceLoggerProvider;
    private readonly IMonitorVolumeView _view;

    public MonitorVolumeController(
        IAudioInterfaceProvider audioInterfaceProvider, 
        IMonitorVolumeView view, 
        IAudioInterfaceLoggerProvider audioInterfaceLoggerProvider)
    {
        _audioInterfaceProvider = audioInterfaceProvider;
        _audioInterfaceLoggerProvider = audioInterfaceLoggerProvider;
        _view = view;
    }

    public string Name => "Monitor volume       : マイクの入力をモニターする。データの保存は行わない。";

    public Task ExecuteAsync()
    {
        using var audioInterface = _audioInterfaceProvider.Resolve();
        using var logger = _audioInterfaceLoggerProvider.ResolveLocal(audioInterface, null);

        CancellationTokenSource source = new();
        logger.StartAsync(source.Token);
        try
        {
            _view.NotifyDetailMessage();
            _view.StartNotify(logger, source.Token);

            _view.WaitToBeStopped();

            return Task.CompletedTask;
        }
        finally
        {
            source.Cancel();
        }
    }
}