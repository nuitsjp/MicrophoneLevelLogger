using System.ComponentModel.DataAnnotations;
using MicrophoneLevelLogger.Client.Controller.CalibrateOutput;
using Sharprompt;

namespace MicrophoneLevelLogger.Client.View;

public class CalibrateOutputView : MicrophoneView, ICalibrateOutputView
{
    public IMicrophone SelectMicrophone(IAudioInterface audioInterface)
    {
        return Prompt.Select(
            "計測対象のマイクを選択してください。",
            audioInterface.Microphones);
    }


    public int InputSpan()
    {
        return Prompt.Input<int>("計測時間[秒]を入力してください。", 30);
    }
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

    public void DisplayOutputVolume(Decibel volume)
    {
        ConsoleEx.WriteLine($" -> 音量 : {volume.AsPrimitive():0.00}");
    }


    public void DisplayDefaultOutputLevel(VolumeLevel level)
    {
        ConsoleEx.Write($"出力レベル : {level.AsPrimitive():0.00}");
    }
}