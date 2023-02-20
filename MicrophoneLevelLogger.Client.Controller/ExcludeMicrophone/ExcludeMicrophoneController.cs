using Sharprompt;

namespace MicrophoneLevelLogger.Client.Controller.ExcludeMicrophone;

public class ExcludeMicrophoneController : IController
{
    private readonly IExcludeMicrophoneView _view;
    private readonly IAudioInterfaceProvider _provider;
    private readonly ISettingsRepository _repository;

    public ExcludeMicrophoneController(
        IExcludeMicrophoneView view, 
        IAudioInterfaceProvider provider, 
        ISettingsRepository repository)
    {
        _view = view;
        _provider = provider;
        _repository = repository;
    }

    public string Name => "Exclude microphone   : 録音を除外するマイクを設定する。";

    public async Task ExecuteAsync()
    {
        var audioInterface = await _provider.ResolveAsync();
        if (_view.TrySelectMicrophone(audioInterface, out var microphone))
        {
            var settings = await _repository.LoadAsync();
            settings.ExcludeMicrophone(microphone.Id);
            await _repository.SaveAsync(settings);
        }

    }
}

public interface IExcludeMicrophoneView
{
    bool TrySelectMicrophone(IAudioInterface audioInterface, out IMicrophone microphone);
}

public class ExcludeMicrophoneView : IExcludeMicrophoneView
{
    public bool TrySelectMicrophone(IAudioInterface audioInterface, out IMicrophone microphone)
    {
        const string cancel = "取りやめる";

        var items = audioInterface.Microphones
            .Select(x => x.Name)
            .ToList();
        items.Add(cancel);

        var selected = Prompt.Select("削除する別名を選択してください。", items);
        if (selected == cancel)
        {
            microphone = default!;
            return false;
        }

        microphone = audioInterface.Microphones[items.IndexOf(selected)];
        return true;
    }
}