using System.ComponentModel.DataAnnotations;
using MicrophoneLevelLogger.Client.Controller.CalibrateOutput;
using Sharprompt;

namespace MicrophoneLevelLogger.Client.View;

/// <summary>
/// スピーカー調整ビュー
/// </summary>
public class CalibrateOutputView : MicrophoneView, ICalibrateOutputView
{
    /// <summary>
    /// 調整に利用するマイクを選択する。
    /// </summary>
    /// <param name="audioInterface"></param>
    /// <returns></returns>
    public IMicrophone SelectMicrophone(IAudioInterface audioInterface)
    {
        return Prompt.Select(
            "計測対象のマイクを選択してください。",
            audioInterface.GetMicrophones());
    }
    /// <summary>
    /// 調整に利用する録音時間を入力する。
    /// </summary>
    /// <returns></returns>

    public int InputSpan()
    {
        return Prompt.Input<int>("計測時間[秒]を入力してください。", 30);
    }
    /// <summary>
    /// スピーカーの調整先の音量を入力する。
    /// </summary>
    /// <returns></returns>
    public Decibel InputDecibel()
    {
        return new(Prompt.Input<double>(
            "調整する出力音量を入力してください（-84～0）",
            null,
            null,
            new List<Func<object, ValidationResult>>
            {
                o =>
                {
                    var decibel = (double) o;
                    return Decibel.Validate(decibel) 
                        ? ValidationResult.Success! 
                        : new ValidationResult("入力音量は-84～0の間で入力してください。");
                }
            }));
    }
    /// <summary>
    /// 計測結果のボリュームを表示する。
    /// </summary>
    /// <param name="volume"></param>
    public void DisplayOutputVolume(Decibel volume)
    {
        ConsoleEx.WriteLine($" -> 音量 : {volume.AsPrimitive():0.00}");
    }
    /// <summary>
    /// スピーカーの音量レベルを表示する。
    /// </summary>
    /// <param name="level"></param>
    public void DisplaySpeakerVolumeLevel(VolumeLevel level)
    {
        ConsoleEx.Write($"出力レベル : {level.AsPrimitive():0.00}");
    }
}