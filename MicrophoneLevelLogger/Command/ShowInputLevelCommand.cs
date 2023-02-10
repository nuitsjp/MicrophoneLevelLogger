using MicrophoneLevelLogger.Domain;

namespace MicrophoneLevelLogger.Command;

public class ShowInputLevelCommand : ICommand
{
    private readonly IShowInputLevelView _view;
    private readonly IAudioInterface _audioInterface;

    public ShowInputLevelCommand(IAudioInterfaceProvider audioInterfaceProvider, IShowInputLevelView view)
    {
        _view = view;
        _audioInterface = audioInterfaceProvider.Resolve();
    }

    public string Name => "Show input           : マイク入力レベルの計測結果を表示する。";


    public async Task ExecuteAsync()
    {
        // 計測結果リストを表示する
        AudioInterfaceInputLevels inputLevels = await AudioInterfaceInputLevels.LoadAsync();
        _view.NotifyResult(inputLevels);
    }
}