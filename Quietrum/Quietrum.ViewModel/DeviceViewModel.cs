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
    private IObservable<WaveInEventArgs>? _observable;
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
        _observable = _device.StartRecording(_recordingConfig.WaveFormat, _recordingConfig.RefreshRate.Interval);
        _bufferedObservable = new BufferedObservable(
            _observable, 
            _recordingConfig.WaveFormat, 
            _recordingConfig.RefreshRate);
        var normalize = new Normalize(_recordingConfig.WaveFormat);
        var fastTimeWeighting = new FastTimeWeighting(_recordingConfig.WaveFormat);
        var fft = new FastFourierTransform(_recordingConfig.WaveFormat);
        var converter = new DecibelConverter();
        _disposable = _bufferedObservable
            .Select(normalize.Filter)
            .Select(fastTimeWeighting.Filter)
            .Select(fft.Transform)
            .Select(converter.Convert)
            .Subscribe(OnNext);
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
        
        _connector = new("localhost", _observable!, (IRenderDevice)_device);
        _connector.Connect();
    }

    private void Disconnect()
    {
        _connector?.Dispose();
        _connector = null;
    }

    public void StopMonitoring()
    {
        _device.StopRecording();
        _disposable?.Dispose();
        _observable = null;
        _bufferedObservable = null;
        Array.Fill(LiveData, Decibel.Minimum.AsPrimitive());
    }

    public void StartRecording(DirectoryInfo directoryInfo)
    {
        if (_observable is null)
        {
            throw new InvalidOperationException();
        }

        var fileInfo = new FileInfo(Path.Combine(directoryInfo.FullName, Name + ".wav"));
        _waveRecorder = new WaveRecorder(
            fileInfo,
            _recordingConfig.WaveFormat,
            _observable);
        _waveRecorder.StartRecording();
    }

    public void StopRecording()
    {
        _waveRecorder?.StopRecording();
        _waveRecorder = null;
    }

    private void OnNext(double decibel)
    {
        // "scroll" the whole chart to the left
        Array.Copy(LiveData, 1, LiveData, 0, LiveData.Length - 1);
        LiveData[^1] = decibel;
        // Minus30dB = $"{_buffer.Count(x => -30d < x) / (double)_buffer.Count * 100d:#0.00}%";
        // Minus40dB = $"{_buffer.Count(x => -40d < x) / (double)_buffer.Count * 100d:#0.00}%";
        // Minus50dB = $"{_buffer.Count(x => -50d < x) / (double)_buffer.Count * 100d:#0.00}%";
    }

    public void Dispose()
    {
        _device.StopRecording();
        _device.Dispose();
    }
}