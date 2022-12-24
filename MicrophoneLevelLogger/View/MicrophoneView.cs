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
        public MicrophoneInfo(int no, string name, MasterVolumeLevelScalar inputLevel)
        {
            No = no;
            Name = name;
            InputLevel = inputLevel;
        }

        public int No { get; }
        public string Name { get; }
        public MasterVolumeLevelScalar InputLevel { get; }
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
            for (var i = 0; i < microphones.Devices.Count; i++)
            {
                var waveInput = microphones.Devices[i].LatestWaveInput;
                Console.WriteLine($"{i + 1} ={waveInput.MaximumDecibel:0.00} {GetBars(waveInput.MaximumDecibel)}");
            }
            Console.SetCursorPosition(0, Console.CursorTop - microphones.Devices.Count);
        }
    }

    private static readonly double MaxBarValue = IMicrophone.MinDecibel * -1;

    private static string GetBars(double decibel, int barCount = 35)
    {
        var value = 
            0 < decibel
                ? MaxBarValue
                : decibel + MaxBarValue;
        var barsOn = (int)(value / MaxBarValue * barCount);
        var barsOff = barCount - barsOn;
        return new string('#', barsOn) + new string('-', barsOff);
    }
}