using System.Windows.Forms.VisualStyles;
using MicrophoneLevelLogger.Domain;

namespace MicrophoneLevelLogger.Command;

public class MeasureCommand : ICommand
{
    private readonly IMeasureView _view;
    private readonly IAudioInterfaceProvider _provider;
    private readonly IMediaPlayer _mediaPlayer;

    public MeasureCommand(
        IMeasureView view, 
        IAudioInterfaceProvider provider, 
        IMediaPlayer mediaPlayer)
    {
        _view = view;
        _provider = provider;
        _mediaPlayer = mediaPlayer;
    }

    public string Name => "Measure              : 指定マイクの入力音量を計測する。";

    public async Task ExecuteAsync()
    {
        // マイクを選択し、有効化する
        var audioInterface = _provider.Resolve();
        var microphone = _view.SelectMicrophone(audioInterface);
        await microphone.ActivateAsync();

        // 計測時間を入力する
        var span = TimeSpan.FromSeconds(_view.InputSpan());

        // 計測値の画面通知を開始する
        _view.StartNotifyMasterPeakValue(new AudioInterface(microphone));

        // 音声を再生する
        await _mediaPlayer.PlayLoopingAsync();

        // 計測を開始する
        var meter = new InputLevelMeter(microphone);
        meter.StartMonitoring();
        try
        {
            // 計測完了を待機する
            await Task.Delay(span);

            // 計測結果を取得する
            var microphoneInputLevel = meter.StopMonitoring();

            // 計測結果リストを更新する
            AudioInterfaceInputLevels inputLevels = await AudioInterfaceInputLevels.LoadAsync();
            inputLevels.Update(microphoneInputLevel);
            await AudioInterfaceInputLevels.SaveAsync(inputLevels);

            // 結果を通知する
            _view.NotifyResult(inputLevels);
        }
        finally
        {
            // リソースを停止・解放する。
            microphone.Deactivate();
            _view.StopNotifyMasterPeakValue();
            await _mediaPlayer.StopAsync();
        }
    }
}

public interface IMeasureView : IMicrophoneView
{
    IMicrophone SelectMicrophone(IAudioInterface audioInterface);
    int InputSpan();
    void NotifyResult(AudioInterfaceInputLevels audioInterfaceInputLevels);
}
