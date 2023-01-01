using MicrophoneLevelLogger.Command;
using MicrophoneLevelLogger.Domain;
using Sharprompt;

namespace MicrophoneLevelLogger.View;

public class CalibrateView : MicrophoneView, ICalibrateView
{
    public IMicrophone SelectReference(IAudioInterface audioInterface)
    {
        return Prompt.Select(
            "リファレンスマイクを選択してください。",
            audioInterface.Microphones);
    }

    public void NotifyCalibrated(IAudioInterface audioInterface)
    {
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine("マイクのキャリブレーションを完了しました。");

        for (var i = 0; i < audioInterface.Microphones.Count; i++)
        {
            var microphone = audioInterface.Microphones[i];
            Console.WriteLine($"{i + 1} = {microphone.Name} 入力レベル：{microphone.MasterVolumeLevelScalar}");
        }
    }
}