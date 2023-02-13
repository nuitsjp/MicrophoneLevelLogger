namespace MicrophoneLevelLogger.Client.Controller.DeleteCalibrates;

public class DeleteCalibratesController : IController
{
    private readonly IDeleteCalibrateView _view;

    public DeleteCalibratesController(IDeleteCalibrateView view)
    {
        _view = view;
    }

    public string Name => "Delete calibrates    : マイク入力レベルの調整結果をすべて削除する。";

    public Task ExecuteAsync()
    {
        if (_view.Confirm())
        {
            AudioInterfaceCalibrationValues.Remove();
        }

        return Task.CompletedTask;
    }
}
