namespace MicrophoneLevelLogger.Client.Controller.DeleteCalibrates;

public class DeleteCalibratesController : IController
{
    private readonly IDeleteCalibrateView _view;
    private readonly IAudioInterfaceCalibrationValuesRepository _audioInterfaceCalibrationValuesRepository;

    public DeleteCalibratesController(IDeleteCalibrateView view, IAudioInterfaceCalibrationValuesRepository audioInterfaceCalibrationValuesRepository)
    {
        _view = view;
        _audioInterfaceCalibrationValuesRepository = audioInterfaceCalibrationValuesRepository;
    }

    public string Name => "Delete calibrates    : マイク入力レベルの調整結果をすべて削除する。";

    public Task ExecuteAsync()
    {
        if (_view.Confirm())
        {
            _audioInterfaceCalibrationValuesRepository.Remove();
        }

        return Task.CompletedTask;
    }
}
