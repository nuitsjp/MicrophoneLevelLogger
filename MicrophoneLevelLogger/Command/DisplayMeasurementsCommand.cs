using MicrophoneLevelLogger.Domain;

namespace MicrophoneLevelLogger.Command;

public class DisplayMeasurementsCommand : ICommand
{
    private readonly IDisplayMeasurementsView _view;

    public DisplayMeasurementsCommand(IDisplayMeasurementsView view)
    {
        _view = view;
    }

    public string Name => "Display measurements : マイク入力音量の計測結果を表示する。";


    public async Task ExecuteAsync()
    {
        // 計測結果リストを表示する
        AudioInterfaceInputLevels inputLevels = await AudioInterfaceInputLevels.LoadAsync();
        _view.NotifyResult(inputLevels);
    }
}