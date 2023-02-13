namespace MicrophoneLevelLogger.Client.Controller.DisplayCalibrates;

public class DisplayCalibratesController : IController
{
    private readonly IDisplayCalibratesView _view;
    private readonly IAudioInterfaceCalibrationValuesRepository _audioInterfaceCalibrationValuesRepository;

    public DisplayCalibratesController(
        IDisplayCalibratesView view, 
        IAudioInterfaceCalibrationValuesRepository audioInterfaceCalibrationValuesRepository)
    {
        _view = view;
        _audioInterfaceCalibrationValuesRepository = audioInterfaceCalibrationValuesRepository;
    }

    public string Name => "Display calibrates   : マイク入力レベルの調整結果を表示する。";


    public async Task ExecuteAsync()
    {
        // 計測結果リストを表示する
        var calibrates = await _audioInterfaceCalibrationValuesRepository.LoadAsync();
        _view.NotifyResult(calibrates);
    }
}