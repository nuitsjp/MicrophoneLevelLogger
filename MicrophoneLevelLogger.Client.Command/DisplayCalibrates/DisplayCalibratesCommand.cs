namespace MicrophoneLevelLogger.Client.Command.DisplayCalibrates;

public class DisplayCalibratesCommand : ICommand
{
    private readonly IDisplayCalibratesView _view;

    public DisplayCalibratesCommand(IDisplayCalibratesView view)
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