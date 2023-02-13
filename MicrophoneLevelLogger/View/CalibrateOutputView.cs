using System.ComponentModel.DataAnnotations;
using MicrophoneLevelLogger.Command;
using MicrophoneLevelLogger.Command.CalibrateOutput;
using MicrophoneLevelLogger.Domain;
using Sharprompt;

namespace MicrophoneLevelLogger.View;

public class CalibrateOutputView : ICalibrateOutputView
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
    public double InputDecibel()
    {
        return Prompt.Input<double>(
            "調整する出力音量を入力してください（-84～0）",
            null,
            null,
            new List<Func<object, ValidationResult>>
            {
                o =>
                {
                    double decibel = (double) o;
                    if (-84 <= decibel && decibel <= 0)
                    {
                        return ValidationResult.Success!;
                    }

                    return new ValidationResult("入力音量は-84～0の間で入力してください。");
                }
            });
    }

    public void DisplayOutputVolume(double volume)
    {
        Console.WriteLine($" -> 音量 : {volume:0.00}");
    }


    public void DisplayDefaultOutputLevel(VolumeLevel level)
    {
        Console.Write($"出力レベル : {level.AsPrimitive():0.00}");
    }
}