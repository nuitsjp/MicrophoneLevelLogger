using System.Reactive.Linq;
using System.Reactive.Subjects;
using CommunityToolkit.Mvvm.ComponentModel;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using MMDeviceEnumerator = NAudio.CoreAudioApi.MMDeviceEnumerator;

namespace Quietrum;

/// <summary>
/// マイク
/// </summary>
public partial class Device : ObservableObject, IDevice
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
    public Device(
        MicrophoneId id,
        string name,
        string systemName,
        bool measure,
        MMDevice mmDevice)
    {
        Id = id;
        Name = name;
        SystemName = systemName;
        _measure = measure;
        _mmDevice = mmDevice;
        _mmDevice.AudioEndpointVolume.OnVolumeNotification += data =>
        {
            OnPropertyChanged(nameof(VolumeLevel));
        };
    }

    /// <summary>
    /// ID
    /// </summary>
    public MicrophoneId Id { get; }

    /// <summary>
    /// 名称
    /// </summary>
    [ObservableProperty] private string _name;
    /// <summary>
    /// Windows上の名称
    /// </summary>
    public string SystemName { get; }

    
    [ObservableProperty] private bool _measure;

    /// <summary>
    /// 入力レベル
    /// </summary>
    public VolumeLevel VolumeLevel
    {
        get => (VolumeLevel)_mmDevice.AudioEndpointVolume.MasterVolumeLevelScalar;
        set
        {
            _mmDevice.AudioEndpointVolume.MasterVolumeLevelScalar = (float)value;
            OnPropertyChanged();
        }
    }

    private WasapiCapture? _wasapiCapture;
    public IObservable<WaveInEventArgs> StartRecording(WaveFormat waveFormat, TimeSpan bufferSpan)
    {
        var subject = new Subject<WaveInEventArgs>();
        _wasapiCapture = new WasapiCapture(_mmDevice)
        {
            WaveFormat = waveFormat,
            ShareMode = AudioClientShareMode.Shared
        };

        _wasapiCapture.DataAvailable += (_, args) =>
        {
            // var buffer = new byte[args.BytesRecorded];
            // Buffer.BlockCopy(args.Buffer, 0, buffer, 0, args.BytesRecorded);
            subject.OnNext(args);
        };
        _wasapiCapture.RecordingStopped += (_, _) =>
        {
            _wasapiCapture?.Dispose();
            _wasapiCapture = null;
            subject.OnCompleted();
        };

        _wasapiCapture.StartRecording();
        return subject.AsObservable();
    }

    public void StopRecording()
    {
        _wasapiCapture?.StopRecording();
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