using MicrophoneLevelLogger.Domain;

namespace MicrophoneLevelLogger.Command;

public class DisplayMeasurementsCommand : ICommand
{
    private readonly IDisplayMeasurementsView _view;
    private readonly IAudioInterface _audioInterface;

    public DisplayMeasurementsCommand(IAudioInterfaceProvider audioInterfaceProvider, IDisplayMeasurementsView view)
    {
        _view = view;
        _audioInterface = audioInterfaceProvider.Resolve();
    }

    public string Name => "Display measurements : マイク入力音量の計測結果を表示する。";


    public async Task ExecuteAsync()
    {
        // 計測結果リストを表示する
        AudioInterfaceInputLevels inputLevels = await AudioInterfaceInputLevels.LoadAsync();
        _view.NotifyResult(inputLevels);
    }
}