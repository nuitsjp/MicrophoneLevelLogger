using MicrophoneNoiseAnalyzer.Domain;
using Sharprompt;

namespace MicrophoneNoiseAnalyzer.View;

public class CalibrationView : ICalibrationView
{
    /// <summary>
    /// 画面に入力レベルを表示する際のサンプリングレート。Microphone側にもあるがあちらは分析用のため別々に定義する。
    /// </summary>
    private static readonly TimeSpan SamplingRate = TimeSpan.FromMilliseconds(50);

    private Timer? _timer;

    public void NotifyMicrophonesInformation(IMicrophones microphones)
    {
        for (int i = 0; i < microphones.Devices.Count; i++)
        {
            var microphone = microphones.Devices[i];
            Console.WriteLine($"{i + 1} = {microphone.Name} 入力レベル：{microphone.MasterVolumeLevelScalar}");
        }
    }

    public bool ConfirmInvoke()
    {
        Console.WriteLine();
        Console.WriteLine("マイクのキャリブレーションを開始します。5秒間、一定の音量の発声をしてください。");
        return Prompt.Confirm("開始してよろしいですか？");
    }

    public void StartNotifyMasterPeakValue(IMicrophones microphones)
    {
        _timer = new Timer(OnElapsed, microphones, TimeSpan.Zero, SamplingRate);
    }

    public void StopNotifyMasterPeakValue()
    {
        _timer?.Change(Timeout.Infinite, Timeout.Infinite);
        _timer?.Dispose();
        _timer = null;
    }

    public void NotifyCalibrated(IMicrophones microphones)
    {
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine("マイクのキャリブレーションを完了しました。");

        for (int i = 0; i < microphones.Devices.Count; i++)
        {
            var microphone = microphones.Devices[i];
            Console.WriteLine($"{i + 1} = {microphone.Name} 入力レベル：{microphone.MasterVolumeLevelScalar}");
        }
    }


    private void OnElapsed(object? state)
    {
        var microphones = (IMicrophones)state!;
        lock (this)
        {
            for (int i = 0; i < microphones.Devices.Count; i++)
            {
                Console.WriteLine($"{i + 1} ={microphones.Devices[i].MasterPeakValue:0.00} {GetBars(microphones.Devices[i].MasterPeakValue)}");
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
}