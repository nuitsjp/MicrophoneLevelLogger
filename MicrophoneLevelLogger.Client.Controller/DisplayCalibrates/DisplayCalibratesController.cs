namespace MicrophoneLevelLogger.Client.Controller.DisplayCalibrates;

public class DisplayCalibratesController : IController
{
    private readonly IDisplayCalibratesView _view;

    public DisplayCalibratesController(IDisplayCalibratesView view)
    {
        _view = view;
    }

    public string Name => "Display calibrates   : マイク入力レベルの調整結果を表示する。";


    public async Task ExecuteAsync()
    {
        // 計測結果リストを表示する
        var calibrates = await AudioInterfaceCalibrationValues.LoadAsync();
        _view.NotifyResult(calibrates);
    }
}