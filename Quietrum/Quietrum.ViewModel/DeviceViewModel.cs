using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reactive.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using Reactive.Bindings.Disposables;
using Reactive.Bindings.Extensions;
using Specter.Business;
using WaveRecorder = Specter.Business.WaveRecorder;

namespace Quietrum.ViewModel;

public partial class DeviceViewModel : ObservableObject, IDisposable
{
    private readonly IDevice _device;
    private readonly RecordingConfig _recordingConfig;
    private IObservable<short[]>? _bufferedObservable;
    private readonly CompositeDisposable _compositeDisposable = new();
    private IDisposable? _disposable;
    private WaveRecorder? _waveRecorder;
    [ObservableProperty] private bool _connected;
    [ObservableProperty] private string _minus30dB = string.Empty;
    [ObservableProperty] private string _minus40dB = string.Empty;
    [ObservableProperty] private string _minus50dB = string.Empty;

    public DeviceViewModel(
        IDevice device,
        RecordingConfig recordingConfig)
    {
        _device = device;
        _device.ObserveProperty(x => x.VolumeLevel)
            .Subscribe(_ => OnPropertyChanged(nameof(VolumeLevel)));
        this.ObserveProperty(x => x.Connected)
            .Skip(1)
            .Subscribe(OnConnected)
            .AddTo(_compositeDisposable);

        _device.InputLevel
            .Subscribe(OnNext);
        _recordingConfig = recordingConfig;
        LiveData = new double[(int)(_recordingConfig.RecordingSpan / _recordingConfig.RefreshRate.Interval)];
        Array.Fill(LiveData, Decibel.Minimum.AsPrimitive());
    }

    private void OnConnected(bool connected)
    {
        if (connected)
        {
            Connect();
        }
        else
        {
            Disconnect();
        }
    }

    public DeviceId Id => _device.Id;
    public DataFlow DataFlow => _device.DataFlow;
    public bool Connectable => DataFlow == DataFlow.Render;

    public string Name
    {
        get => _device.Name;
        set
        {
            _device.Name = value;
            OnPropertyChanged();
        }
    }

    public string SystemName => _device.SystemName;

    /// <summary>
    /// 入力レベル
    /// </summary>
    public string VolumeLevel
    {
        get => (_device.VolumeLevel.AsPrimitive() * 100).ToString("0");
        set
        {
            if(int.TryParse(value, out var intValue))
            {
                if (intValue is >= 0 and <= 100)
                {
                    _device.VolumeLevel = new VolumeLevel(intValue / 100f);
                }
            }
            OnPropertyChanged();
        }
    }

    public bool Measure
    {
        get => _device.Measure;
        set
        {
            _device.Measure = value;
            if (_device.Measure)
            {
                StartMonitoring();
            }
            else
            {
                StopMonitoring();
            }
            OnPropertyChanged();
        }
    }
    public double[] LiveData { get; }

    public void StartMonitoring()
    {
        _device.StartMonitoring(_recordingConfig.WaveFormat, _recordingConfig.RefreshRate);
    }

    public Task PlayLoopingAsync(CancellationToken token)
    {
        return ((IRenderDevice)_device).PlayLoopingAsync(token);
    }

    private RemoteDeviceConnector? _connector;

    private void Connect()
    {
        if (Measure is false)
        {
            Measure = true;
        }
        
        _connector = new("localhost", (IRenderDevice)_device);
        _connector.Connect();
    }

    private void Disconnect()
    {
        _connector?.Dispose();
        _connector = null;
    }

    public void StopMonitoring()
    {
        _device.StopMonitoring();
        _disposable?.Dispose();
        _bufferedObservable = null;
        Array.Fill(LiveData, Decibel.Minimum.AsPrimitive());
    }

    private static readonly Decibel Minus30Decibel = new(-30d);
    private static readonly Decibel Minus40Decibel = new(-40d);
    private static readonly Decibel Minus50Decibel = new(-50d);
    public void StartRecording(DirectoryInfo directoryInfo)
    {
        var fileInfo = new FileInfo(Path.Combine(directoryInfo.FullName, Name + ".wav"));
        _waveRecorder = new WaveRecorder(
            _device,
            fileInfo,
            _recordingConfig.WaveFormat);
        _waveRecorder.StartRecording();

        _device.InputLevel
            .Scan(0, (acc, _) => acc + 1) // カウントを増やす
            .Where(count => count % 10 == 0) // 10で割り切れる場合のみ処理を行う
            .Subscribe(_ =>
            {
                Minus30dB = $"{_waveRecorder.Decibels.Count(x => Minus30Decibel <= x) / (double)_waveRecorder.Decibels.Count * 100d:#0.00}%";
                Minus40dB = $"{_waveRecorder.Decibels.Count(x => Minus40Decibel <= x) / (double)_waveRecorder.Decibels.Count * 100d:#0.00}%";
                Minus50dB = $"{_waveRecorder.Decibels.Count(x => Minus50Decibel <= x) / (double)_waveRecorder.Decibels.Count * 100d:#0.00}%";
            })
            .AddTo(_compositeDisposable);
    }

    public void StopRecording()
    {
        _waveRecorder?.StopRecording();
        _waveRecorder = null;
    }

    private void OnNext(Decibel decibel)
    {
        // "scroll" the whole chart to the left
        Array.Copy(LiveData, 1, LiveData, 0, LiveData.Length - 1);
        LiveData[^1] = decibel.AsPrimitive();
    }

    public void Dispose()
    {
        _device.StopMonitoring();
        _device.Dispose();
    }
}