namespace MicrophoneLevelLogger.Client.Controller.DeleteInputLevels;

public class DeleteInputLevelsController : IController
{
    private readonly IDeleteInputLevelsView _view;
    private readonly IAudioInterfaceInputLevelsRepository _repository;
    public DeleteInputLevelsController(
        IDeleteInputLevelsView view, 
        IAudioInterfaceInputLevelsRepository repository)
    {
        _view = view;
        _repository = repository;
    }

    public string Name => "Delete measurements  : マイク入力音量の計測結果をすべて削除する。";


    public Task ExecuteAsync()

    {
        if (_view.Confirm())
        {
            _repository.Remove();
        }
        return Task.CompletedTask;
    }
}