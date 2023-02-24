namespace MicrophoneLevelLogger.Client.Controller.DeleteCalibrates;

/// <summary>
/// マイク入力レベルの調整結果を削除する。
/// </summary>
public class DeleteCalibratesController : IController
{
    /// <summary>
    /// ビュー
    /// </summary>
    private readonly IDeleteCalibrateView _view;
    /// <summary>
    /// IAudioInterfaceCalibrationValuesリポジトリー
    /// </summary>
    private readonly IAudioInterfaceCalibrationValuesRepository _audioInterfaceCalibrationValuesRepository;

    /// <summary>
    /// インスタンスを生成する。
    /// </summary>
    /// <param name="view"></param>
    /// <param name="audioInterfaceCalibrationValuesRepository"></param>
    public DeleteCalibratesController(
        IDeleteCalibrateView view, 
        IAudioInterfaceCalibrationValuesRepository audioInterfaceCalibrationValuesRepository)
    {
        _view = view;
        _audioInterfaceCalibrationValuesRepository = audioInterfaceCalibrationValuesRepository;
    }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name => "Delete calibrates";
    /// <summary>
    /// 概要
    /// </summary>
    public string Description => "マイク入力レベルの調整結果をすべて削除する。";

    public Task ExecuteAsync()
    {
        // 削除してよいか確認する。
        if (_view.Confirm())
        {
            // 調整結果を削除する。
            _audioInterfaceCalibrationValuesRepository.Remove();
        }

        return Task.CompletedTask;
    }
}
