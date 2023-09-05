using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CommunityToolkit.Mvvm.ComponentModel;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace Specter.Business;

/// <summary>
/// マイク
/// </summary>
public abstract partial class Device : ObservableObject, IDevice
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
    protected Device(
        DeviceId id,
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
    public DeviceId Id { get; }

    /// <summary>
    /// DataFlow
    /// </summary>
    public abstract DataFlow DataFlow { get; }

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

    private IWaveIn? _waveIn;
    private IDisposable? _waveInUnsubscribe;
    private readonly Subject<WaveInEventArgs> _waveInput = new();
    public IObservable<WaveInEventArgs> WaveInput => _waveInput.AsObservable();
    private readonly Subject<Decibel> _inputLevel = new();
    public IObservable<Decibel> InputLevel => _inputLevel.AsObservable();

    public void StartMonitoring(WaveFormat waveFormat, RefreshRate refreshRate)
    {
        _waveIn =
            DataFlow == DataFlow.Capture
                ? new WasapiCapture(_mmDevice)
                {
                    WaveFormat = waveFormat,
                    ShareMode = AudioClientShareMode.Shared
                }
                : new WasapiLoopbackCapture(_mmDevice)
                {
                    WaveFormat = waveFormat,
                    ShareMode = AudioClientShareMode.Shared
                };
        
        _waveIn.DataAvailable += (_, args) =>
        {
            // var buffer = new byte[args.BytesRecorded];
            // Buffer.BlockCopy(args.Buffer, 0, buffer, 0, args.BytesRecorded);
            _waveInput.OnNext(args);
        };
        _waveIn.RecordingStopped += (_, _) =>
        {
            _waveIn?.Dispose();
            _waveIn = null;
        };

        try
        {
            _waveIn.StartRecording();
        }
        catch
        {
            // ignore
        }

        _waveInUnsubscribe =
            new WaveInToInputLevelObservable(this, waveFormat, refreshRate)
                .Subscribe(x => _inputLevel.OnNext(x));
    }

    public void StopMonitoring()
    {
        _waveIn?.StopRecording();
        _waveInUnsubscribe?.Dispose();
    }

    /// <summary>
    /// 文字列表現を取得する。
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Name;

    public void Dispose()
    {
        _waveInput.OnCompleted();
        _inputLevel.OnCompleted();
        _mmDevice.Dispose();
    }
}