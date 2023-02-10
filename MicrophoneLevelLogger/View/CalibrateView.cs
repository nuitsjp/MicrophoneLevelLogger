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

    public IMicrophone SelectTarget(IAudioInterface audioInterface, IMicrophone reference)
    {
        while (true)
        {
            var target =
                Prompt.Select(
                    "キャリブレーション対象のマイクを選択してください。",
                    audioInterface.Microphones);

            if (target.Id != reference.Id)
            {
                return target;
            }
            Console.WriteLine();
            Console.WriteLine("リファレンスとは異なるマイクを選択してください。");
            Console.WriteLine();
        }
    }

    public void NotifyCalibrated(IAudioInterface audioInterface)
    {
        lock (this)
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
}