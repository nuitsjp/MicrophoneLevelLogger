using FluentTextTable;
using MicrophoneLevelLogger.Domain;

namespace MicrophoneLevelLogger.View;

public abstract class MicrophoneView : IMicrophoneView
{
    /// <summary>
    /// 画面に入力レベルを表示する際のサンプリングレート。Microphone側にもあるがあちらは分析用のため別々に定義する。
    /// </summary>
    private static readonly TimeSpan SamplingRate = TimeSpan.FromMilliseconds(50);

    private Timer? _timer;

    public void NotifyMicrophonesInformation(IMicrophones microphones)
    {
        var infos = microphones
            .Devices
            .Select((x, index) => new MicrophoneInfo(index + 1, x.Name, x.MasterVolumeLevelScalar))
            .ToList();
        Build
            .TextTable<MicrophoneInfo>(builder =>
            {
                builder.Borders.InsideHorizontal.AsDisable();
            })
            .WriteLine(infos);

        Console.WriteLine();
    }

    public class MicrophoneInfo
    {
        public MicrophoneInfo(int no, string name, float inputLevel)
        {
            No = no;
            Name = name;
            InputLevel = inputLevel;
        }

        public int No { get; }
        public string Name { get; }
        public float InputLevel { get; }
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