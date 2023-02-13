namespace MicrophoneLevelLogger.Client.Controller.DisplayMeasurements;

public class DisplayMeasurementsController : IController
{
    private readonly IDisplayMeasurementsView _view;
    private readonly IAudioInterfaceInputLevelsRepository _repository;

    public DisplayMeasurementsController(
        IDisplayMeasurementsView view, 
        IAudioInterfaceInputLevelsRepository repository)
    {
        _view = view;
        _repository = repository;
    }

    public string Name => "Display measurements : マイク入力音量の計測結果を表示する。";


    public async Task ExecuteAsync()
    {
        // 計測結果リストを表示する
        AudioInterfaceInputLevels inputLevels = await _repository.LoadAsync();
        _view.NotifyResult(inputLevels);
    }
}