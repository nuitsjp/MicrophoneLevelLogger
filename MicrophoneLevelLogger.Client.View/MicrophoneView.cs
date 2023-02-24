using FluentTextTable;
using MicrophoneLevelLogger.Client.Controller;

namespace MicrophoneLevelLogger.Client.View;

/// <summary>
/// 汎用ビュー
/// </summary>
public class MicrophoneView : IMicrophoneView
{
    /// <summary>
    /// 画面に入力レベルを表示する際のサンプリングレート。Microphone側にもあるがあちらは分析用のため別々に定義する。
    /// </summary>
    private static readonly TimeSpan SamplingRate = TimeSpan.FromMilliseconds(50);

    /// <summary>
    /// オーディオインターフェースの状態を通知する。
    /// </summary>
    /// <param name="audioInterface"></param>
    /// <returns></returns>
    public async Task NotifyAudioInterfaceAsync(IAudioInterface audioInterface)
    {
        var infos = audioInterface
            .GetMicrophones()
            .Select((x, index) => new MicrophoneInfo(index + 1, x.Name, x.VolumeLevel))
            .ToList();
        Build
            .TextTable<MicrophoneInfo>(builder =>
            {
                builder.Borders.InsideHorizontal.AsDisable();
            })
            .WriteLine(infos);

        // ReSharper disable once LocalizableElement
        Console.WriteLine($"スピーカー : {(await audioInterface.GetSpeakerAsync()).Name}");

        ConsoleEx.WriteLine();
    }

    /// <summary>
    /// レコーダーの記録状況の通知を開始する。
    /// </summary>
    /// <param name="recorder"></param>
    /// <param name="token"></param>
    public void StartNotify(IRecorder recorder, CancellationToken token)
    {
        Task.Run( async () =>
        {

            var microphones = recorder.MicrophoneRecorders;

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

    /// <summary>
    /// 記録結果を通知する。
    /// </summary>
    /// <param name="logger"></param>
    public void NotifyResult(IRecorder logger)
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
                    logger.MicrophoneRecorders
                        .Select((x, index) => new RecordResult(index, x)));
        }
    }

    /// <summary>
    /// Enterでキャンセルできる状態で、指定時間待機する。
    /// </summary>
    /// <param name="timeSpan"></param>
    public void Wait(TimeSpan timeSpan)
    {
        ConsoleEx.Wait(timeSpan);
    }

    /// <summary>
    /// 音量バーが最大を表すときの値
    /// </summary>
    private static readonly double MaxBarValue = Decibel.Minimum.AsPrimitive() * -1;

    /// <summary>
    /// 音量バーを取得する
    /// </summary>
    /// <param name="decibel"></param>
    /// <param name="barCount"></param>
    /// <returns></returns>
    private static string GetBars(Decibel decibel, int barCount = 35)
    {
        var value =
            Decibel.Maximum < decibel
                ? MaxBarValue
                : decibel.AsPrimitive() + MaxBarValue;
        var barsOn = (int)(value / MaxBarValue * barCount);
        var barsOff = barCount - barsOn;
        return new string('#', barsOn) + new string('-', barsOff);
    }
    private class MicrophoneInfo
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
}