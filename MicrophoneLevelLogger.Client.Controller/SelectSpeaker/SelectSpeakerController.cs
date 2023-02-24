namespace MicrophoneLevelLogger.Client.Controller.SelectSpeaker;

public class SelectSpeakerController : IController
{
    private readonly ISelectSpeakerView _view;
    private readonly IAudioInterfaceProvider _provider;
    private readonly ISettingsRepository _repository;

    public SelectSpeakerController(
        ISelectSpeakerView view, 
        IAudioInterfaceProvider provider, 
        ISettingsRepository repository)
    {
        _view = view;
        _provider = provider;
        _repository = repository;
    }

    public string Name => "Select speaker";
    public string Description => "音源を再生するスピーカーを選択する。";

    public async Task ExecuteAsync()
    {
        var audioInterface = _provider.Resolve();
        if (_view.TrySelectSpeaker(
                audioInterface.GetSpeakers(), 
                await audioInterface.GetSpeakerAsync(),
                out var selected))
        {
            var settings = await _repository.LoadAsync();
            await _repository.SaveAsync(
                new Settings(
                    settings.MediaPlayerHost,
                    settings.RecorderHost,
                    settings.RecordingSpan,
                    settings.IsEnableRemotePlaying,
                    settings.IsEnableRemoteRecording,
                    settings.Aliases,
                    settings.DisabledMicrophones,
                    selected.Id));
        }
    }
}