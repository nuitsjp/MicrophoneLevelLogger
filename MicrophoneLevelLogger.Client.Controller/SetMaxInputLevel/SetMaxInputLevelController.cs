namespace MicrophoneLevelLogger.Client.Controller.SetMaxInputLevel;

/// <summary>
/// マイクの入力レベルを最大値にする。
/// </summary>
public class SetMaxInputLevelController : IController
{
    /// <summary>
    /// IAudioInterfaceプロバイダー
    /// </summary>
    private readonly IAudioInterfaceProvider _audioInterfaceProvider;

    /// <summary>
    /// インスタンスを生成する。
    /// </summary>
    /// <param name="audioInterfaceProvider"></param>
    public SetMaxInputLevelController(IAudioInterfaceProvider audioInterfaceProvider)
    {
        _audioInterfaceProvider = audioInterfaceProvider;
    }

    /// <summary>
    /// 別名
    /// </summary>
    public string Name => "Set Max";
    /// <summary>
    /// 概要
    /// </summary>
    public string Description => "マイクを入力レベルを最大に変更する。";

    public Task ExecuteAsync()
    {
        // すべてのマイクの入力レベルを最大値に設定する。
        var audioInterface = _audioInterfaceProvider.Resolve();
        foreach (var microphone in audioInterface.GetMicrophones())
        {
            microphone.VolumeLevel = VolumeLevel.Maximum;
        }

        return Task.CompletedTask;
    }
}