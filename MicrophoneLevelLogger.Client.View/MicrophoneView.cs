using FluentTextTable;
using MicrophoneLevelLogger.Client.Controller;
using static System.Windows.Forms.AxHost;

namespace MicrophoneLevelLogger.Client.View;

public class MicrophoneView : IMicrophoneView
{
    /// <summary>
    /// 画面に入力レベルを表示する際のサンプリングレート。Microphone側にもあるがあちらは分析用のため別々に定義する。
    /// </summary>
    private static readonly TimeSpan SamplingRate = TimeSpan.FromMilliseconds(50);

    private Timer? _timer;

    public void NotifyMicrophonesInformation(IAudioInterface audioInterface)
    {
        var infos = audioInterface
            .Microphones
            .Select((x, index) => new MicrophoneInfo(index + 1, x.Name, x.VolumeLevel))
            .ToList();
        Build
            .TextTable<MicrophoneInfo>(builder =>
            {
                builder.Borders.InsideHorizontal.AsDisable();
            })
            .WriteLine(infos);

        ConsoleEx.WriteLine();
    }

    public class MicrophoneInfo
    {
        public MicrophoneInfo(int no, string name, VolumeLevel inputLevel)
        {
            No = no;
            Name = name;
            InputLevel = inputLevel;
        }

        public int No { get; }
        public string Name { get; }
        public VolumeLevel InputLevel { get; }
    }
    public void StartNotifyMasterPeakValue(IAudioInterface audioInterface)
    {
        _timer = new Timer(OnElapsed, audioInterface, TimeSpan.Zero, SamplingRate);
    }

    public void StopNotifyMasterPeakValue()
    {
        _timer?.Change(Timeout.Infinite, Timeout.Infinite);
        _timer?.Dispose();
        _timer = null;
    }

    public void StartNotify(IAudioInterfaceLogger audioInterfaceLogger, CancellationToken token)
    {
        Task.Run( async () =>
        {

            var microphones = audioInterfaceLogger.MicrophoneLoggers;

            while (token.IsCancellationRequested is false)
            {
                lock (this)
                {
                    for (var i = 0; i < microphones.Count; i++)
                    {
                        var microphoneLogger = microphones[i];
                        ConsoleEx.WriteLine($"{i + 1} ={microphoneLogger.Max.AsPrimitive():0.00} {GetBars(microphoneLogger.Max)}");
                    }
                    ConsoleEx.SetCursorPosition(0, ConsoleEx.CursorTop - microphones.Count);
                }

                try
                {
                    // サンプリング間隔待機する。
                    await Task.Delay(SamplingRate, token);
                }
                catch (TaskCanceledException)
                {
                }

            }
        }, token);
    }

    public void NotifyResult(IAudioInterfaceLogger logger)
    {
        lock (this)
        {
            Build
                .TextTable<RecordResult>(builder =>
                {
                    builder.Borders.InsideHorizontal.AsDisable();
                    builder.Columns.Add(x => x.No);
                    builder.Columns.Add(x => x.Name);
                    builder.Columns.Add(x => x.Min).FormatAs("{0:#.00}");
                    builder.Columns.Add(x => x.Avg).FormatAs("{0:#.00}");
                    builder.Columns.Add(x => x.Max).FormatAs("{0:#.00}");
                })
                .WriteLine(
                    logger.MicrophoneLoggers
                        .Select((x, index) => new RecordResult(index, x)));
        }
    }

    public void Wait(TimeSpan timeSpan)
    {
        ConsoleEx.Wait(timeSpan);
    }


    private void OnElapsed(object? state)
    {
        var microphones = (IAudioInterface)state!;
        lock (this)
        {
            for (var i = 0; i < microphones.Microphones.Count; i++)
            {
                var waveInput = microphones.Microphones[i].LatestWaveInput;
                ConsoleEx.WriteLine($"{i + 1} ={waveInput.MaximumDecibel:0.00} {GetBars(waveInput.MaximumDecibel)}");
            }
            ConsoleEx.SetCursorPosition(0, ConsoleEx.CursorTop - microphones.Microphones.Count);
        }
    }

    private static readonly double MaxBarValue = Decibel.Min.AsPrimitive() * -1;

    private static string GetBars(Decibel decibel, int barCount = 35)
    {
        var value =
            Decibel.Max < decibel
                ? MaxBarValue
                : decibel.AsPrimitive() + MaxBarValue;
        var barsOn = (int)(value / MaxBarValue * barCount);
        var barsOff = barCount - barsOn;
        return new string('#', barsOn) + new string('-', barsOff);
    }

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