using System.Reactive.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using NAudio.CoreAudioApi;
using Reactive.Bindings.Disposables;
using Reactive.Bindings.Extensions;
using Specter.Business;
using WaveRecorder = Specter.Business.WaveRecorder;

namespace Quietrum.ViewModel;

public partial class DeviceViewModel : ObservableObject, IDisposable
{
    private readonly IDevice _device;
    private readonly RecordingConfig _recordingConfig;
    private readonly IWaveRecordIndexRepository _indexRepository;
    private readonly CompositeDisposable _compositeDisposable = new();
    private WaveRecorder? _waveRecorder;
    [ObservableProperty] private bool _connected;

    public DeviceViewModel(
        IDevice device,
        RecordingConfig recordingConfig, 
        IWaveRecordIndexRepository indexRepository)
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
        _indexRepository = indexRepository;
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
        Array.Fill(LiveData, Decibel.Minimum.AsPrimitive());
    }

    public void StartRecording(DirectoryInfo directoryInfo)
    {
        _waveRecorder = new WaveRecorder(
            _device,
            _recordingConfig.WaveFormat,
            new DirectoryInfo(Path.Combine(directoryInfo.FullName, Name)),
            _indexRepository);
        _waveRecorder.StartRecording();
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