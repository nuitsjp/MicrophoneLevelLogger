using MicrophoneLevelLogger.Command;
using MicrophoneLevelLogger.Domain;
using Sharprompt;

namespace MicrophoneLevelLogger.View;

public class CalibrateView : MicrophoneView, ICalibrateView
{
    public IMicrophone SelectReference(IMicrophones microphones)
    {
        return Prompt.Select(
            "リファレンスマイクを選択してください。",
            microphones.Devices);
    }

    public void NotifyCalibrated(IMicrophones microphones)
    {
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine("マイクのキャリブレーションを完了しました。");

        for (var i = 0; i < microphones.Devices.Count; i++)
        {
            var microphone = microphones.Devices[i];
            Console.WriteLine($"{i + 1} = {microphone.Name} 入力レベル：{microphone.MasterVolumeLevelScalar}");
        }
    }
}