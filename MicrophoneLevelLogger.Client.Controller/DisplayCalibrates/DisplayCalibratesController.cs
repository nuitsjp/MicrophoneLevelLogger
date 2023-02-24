namespace MicrophoneLevelLogger.Client.Controller.DisplayCalibrates;

/// <summary>
/// マイクの調整結果を表示する。
/// </summary>
public class DisplayCalibratesController : IController
{
    /// <summary>
    /// ビュー
    /// </summary>
    private readonly IDisplayCalibratesView _view;
    /// <summary>
    /// IAudioInterfaceCalibrationValuesリポジトリー
    /// </summary>
    private readonly IAudioInterfaceCalibrationValuesRepository _audioInterfaceCalibrationValuesRepository;

    /// <summary>
    /// インスタンスを生成する。
    /// </summary>
    /// <param name="view"></param>
    /// <param name="audioInterfaceCalibrationValuesRepository"></param>
    public DisplayCalibratesController(
        IDisplayCalibratesView view, 
        IAudioInterfaceCalibrationValuesRepository audioInterfaceCalibrationValuesRepository)
    {
        _view = view;
        _audioInterfaceCalibrationValuesRepository = audioInterfaceCalibrationValuesRepository;
    }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name => "Display calibrates";
    /// <summary>
    /// 概要
    /// </summary>
    public string Description => "マイク入力レベルの調整結果を表示する。";


    public async Task ExecuteAsync()
    {
        // 計測結果リストを表示する
        var calibrates = await _audioInterfaceCalibrationValuesRepository.LoadAsync();
        _view.NotifyResult(calibrates);
    }
}