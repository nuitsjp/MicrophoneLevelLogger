using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace Quietrum;

/// <summary>
/// マイクのレコーダー
/// </summary>
public class MicrophoneRecorder : IMicrophoneRecorder
{
    /// <summary>
    /// サンプリング間隔
    /// </summary>
    public static readonly TimeSpan SamplingSpan = TimeSpan.FromMilliseconds(125);
    /// <summary>
    /// 録音ファイルの保管ディレクトリ。nullの場合、保管しない。
    /// </summary>
    private readonly DirectoryInfo? _directoryInfo;

    /// <summary>
    /// 履歴
    /// </summary>
    private readonly List<Decibel> _avgHistory = new();

    /// <summary>
    /// インスタンスを生成する。
    /// </summary>
    /// <param name="device"></param>
    /// <param name="directoryInfo">録音ファイルの保管ディレクトリ。nullの場合、保管しない。</param>
    public MicrophoneRecorder(IDevice device, DirectoryInfo? directoryInfo)
    {
        Device = device;
        _directoryInfo = directoryInfo;

    }

    /// <summary>
    /// マイク
    /// </summary>
    public IDevice Device { get; }
    /// <summary>
    /// サンプリング間隔中の最大音量。
    /// </summary>
    public Decibel Max { get; private set; } = Decibel.Minimum;
    /// <summary>
    /// サンプリング間隔中の平均音量
    /// </summary>
    public Decibel Avg { get; private set; } = Decibel.Minimum;
    /// <summary>
    /// サンプリング間隔中の最小音量
    /// </summary>
    public Decibel Min { get; private set; } = Decibel.Minimum;

    /// <summary>
    /// 録音を開始する。
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task StartAsync(CancellationToken token)
    {
        var waveInEvent = new WaveInEvent
        {
            DeviceNumber = GetDeviceNumber(),
            WaveFormat = new WaveFormat(rate: 48_000, bits: 16, channels: 1),
            BufferMilliseconds = (int)SamplingSpan.TotalMilliseconds
        };
        Fft fft = new(waveInEvent.WaveFormat);
        var waveWriter = _directoryInfo is not null
            ? new WaveFileWriter(Path.Join(_directoryInfo.FullName, $"{Device.Name}.wav"), waveInEvent.WaveFormat)
            : Stream.Null;

        // マイクからの入力を受け取る。
        waveInEvent.DataAvailable += (_, e) =>
        {
            // オリジナルの入力音源を保存する。
            waveWriter.Write(e.Buffer, 0, e.BytesRecorded);
            // 高速フーリエ変換→A特性の適用
            var decibels =
                AWeighting.Instance.Filter(
                    fft.Transform(e.Buffer, e.BytesRecorded));
            // 最新の入力の平均値を取得し、履歴に追加し、トータルの平均を求める。
            var avg = (Decibel)decibels.Average(x => x.Decibel.AsPrimitive());
            _avgHistory.Add(avg);
            // サンプリング間隔中の各値を記録する。
            Min = decibels.Min(x => x.Decibel);
            Avg = (Decibel)_avgHistory.Average(x => x.AsPrimitive());
            Max = decibels.Max(x => x.Decibel);
        };

        // キャンセル処理を登録する。
        token.Register(() =>
        {
            // レコーディングを停止し、リソースを開放する。
            waveInEvent.StopRecording();
            waveInEvent.Dispose();
            waveWriter.Flush();
            waveWriter.Dispose();
        });

        // レコーディングを開始する。
        waveInEvent.StartRecording();

        return Task.CompletedTask;
    }
    
    private int GetDeviceNumber()
    {
        using MMDeviceEnumerator deviceEnumerator = new MMDeviceEnumerator();
        MMDeviceCollection devices = deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);

        for (int i = 0; i < devices.Count; i++)
        {
            if (devices[i].ID == Device.Id.AsPrimitive())
            {
                return i;
            }
        }

        throw new NotImplementedException("デバイスが処理中に抜かれた場合の対応は未実装です。");
    }
}