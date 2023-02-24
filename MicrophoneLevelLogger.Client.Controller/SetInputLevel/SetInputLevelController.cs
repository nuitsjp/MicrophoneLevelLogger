namespace MicrophoneLevelLogger.Client.Controller.SetInputLevel;

/// <summary>
/// マイクの入力レベルを設定する。
/// </summary>
public class SetInputLevelController : IController
{
    /// <summary>
    /// ビュー
    /// </summary>
    private readonly ISetInputLevelView _view;
    /// <summary>
    /// IAudioInterfaceプロバイダー
    /// </summary>
    private readonly IAudioInterfaceProvider _audioInterfaceProvider;

    /// <summary>
    /// インスタンスを生成する。
    /// </summary>
    /// <param name="view"></param>
    /// <param name="audioInterfaceProvider"></param>
    public SetInputLevelController(
        ISetInputLevelView view,
        IAudioInterfaceProvider audioInterfaceProvider)
    {
        _view = view;
        _audioInterfaceProvider = audioInterfaceProvider;
    }
    /// <summary>
    /// 名称
    /// </summary>
    public string Name => "Set input level";
    /// <summary>
    /// 概要
    /// </summary>
    public string Description => "指定マイクを入力レベルを変更する。";

    public async Task ExecuteAsync()
    {
        var audioInterface = _audioInterfaceProvider.Resolve();

        // マイクを選択し、入力レベルを入力する。
        var microphone = _view.SelectMicrophone(audioInterface);
        var inputLevel = _view.InputInputLevel();

        // 入力レベルを設定する。
        microphone.VolumeLevel = new(inputLevel);

        // 入力結果を表示する。
        await _view.NotifyAudioInterfaceAsync(audioInterface);
    }
}