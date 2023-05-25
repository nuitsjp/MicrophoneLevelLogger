using System.Reactive.Linq;
using System.Reactive.Subjects;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using MMDeviceEnumerator = NAudio.CoreAudioApi.MMDeviceEnumerator;

namespace Quietrum;

/// <summary>
/// マイク
/// </summary>
public class Microphone : IMicrophone
{
    private readonly MMDevice _mmDevice;

    /// <summary>
    /// インスタンスを生成する。
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="systemName"></param>
    /// <param name="measure"></param>
    /// <param name="mmDevice"></param>
    public Microphone(
        MicrophoneId id,
        string name,
        string systemName,
        bool measure,
        MMDevice mmDevice)
    {
        Id = id;
        Name = name;
        SystemName = systemName;
        Measure = measure;
        _mmDevice = mmDevice;
    }

    /// <summary>
    /// ID
    /// </summary>
    public MicrophoneId Id { get; }
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// Windows上の名称
    /// </summary>
    public string SystemName { get; }
    public bool Measure { get; set; }

    /// <summary>
    /// 入力レベル
    /// </summary>
    public VolumeLevel VolumeLevel
    {
        get
        {
            using var enumerator = new MMDeviceEnumerator();
            using var mmDevice = enumerator.GetDevice(Id.AsPrimitive());
            return (VolumeLevel)mmDevice.AudioEndpointVolume.MasterVolumeLevelScalar;
        }
        set
        {
            using var enumerator = new MMDeviceEnumerator();
            using var mmDevice = enumerator.GetDevice(Id.AsPrimitive());
            mmDevice.AudioEndpointVolume.MasterVolumeLevelScalar = (float)value;
        }
    }

    private int GetDeviceNumber()
    {
        using MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
        var mmDevices = enumerator
            .EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active)
            .ToArray();

        for (int i = 0; i < mmDevices.Length; i++)
        {
            if (mmDevices[i].ID == Id.AsPrimitive())
            {
                return i;
            }
        }

        throw new NotImplementedException("デバイスが処理中に抜かれた場合の対応は未実装です。");
    }
    public IObservable<byte[]> StartRecording(WaveFormat waveFormat, TimeSpan bufferSpan, CancellationToken cancellationToken)
    {
        var subject = new Subject<byte[]>();
        WasapiCapture capture = new WasapiCapture(_mmDevice)
        {
            WaveFormat = waveFormat,
            ShareMode = AudioClientShareMode.Shared
        };
        

        capture.DataAvailable += (_, args) =>
        {
            var buffer = new byte[args.BytesRecorded];
            Buffer.BlockCopy(args.Buffer, 0, buffer, 0, args.BytesRecorded);
            subject.OnNext(buffer);
        };
        capture.RecordingStopped += (sender, args) =>
        {
            capture.Dispose();
            subject.OnCompleted();
        };
        cancellationToken.Register(() =>
        {
            capture.StopRecording();
        });

        capture.StartRecording();
        return subject.AsObservable();
    }

    /// <summary>
    /// 文字列表現を取得する。
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Name;

    public void Dispose()
    {
        _mmDevice.Dispose();
    }
}