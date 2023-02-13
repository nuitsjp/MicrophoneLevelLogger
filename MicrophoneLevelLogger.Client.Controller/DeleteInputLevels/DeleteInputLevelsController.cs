namespace MicrophoneLevelLogger.Client.Controller.DeleteInputLevels;

public class DeleteInputLevelsController : IController
{
    private readonly IDeleteInputLevelsView _view;
    public DeleteInputLevelsController(IDeleteInputLevelsView view)
    {
        _view = view;
    }

    public string Name => "Delete measurements  : マイク入力音量の計測結果をすべて削除する。";


    public Task ExecuteAsync()

    {
        if (_view.Confirm())
        {
            AudioInterfaceInputLevels.Remove();
        }
        return Task.CompletedTask;
    }
}