using MicrophoneNoiseAnalyzer.Domain;
using Sharprompt;

namespace MicrophoneNoiseAnalyzer.View;

public class CalibrationView : ICalibrationView
{
    public void NotifyMicrophonesInformation(IMicrophones microphones)
    {
        for (int i = 0; i < microphones.Devices.Count; i++)
        {
            var microphone = microphones.Devices[i];
            Console.WriteLine($"{i + 1} = {microphone.Name} {microphone.MasterVolumeLevelScalar}");
        }
    }

    public bool ConfirmInvoke()
    {
        Console.WriteLine("");
        Console.WriteLine("マイクのキャリブレーションを開始します。5秒間、一定の音量の発声をしてください。");
        return Prompt.Confirm("開始してよろしいですか？");
    }

    public void NotifyMasterPeakValue(IMicrophones microphones)
    {
        lock (this)
        {
            for (int i = 0; i < microphones.Devices.Count; i++)
            {
                Console.WriteLine($"{i + 1} ={GetBars(microphones.Devices[i].MasterPeakValue)}");
            }
            Console.SetCursorPosition(0, Console.CursorTop - microphones.Devices.Count);
        }
    }

    private static string GetBars(double fraction, int barCount = 35)
    {
        var barsOn = (int)(barCount * fraction);
        var barsOff = barCount - barsOn;
        return new string('#', barsOn) + new string('-', barsOff);
    }

    public static void ClearCurrentConsoleLine()
    {
        int currentLineCursor = Console.CursorTop;
        Console.SetCursorPosition(0, Console.CursorTop);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, currentLineCursor);
    }
}