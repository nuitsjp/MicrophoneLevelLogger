using System.Reactive.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using NAudio.CoreAudioApi;
using Reactive.Bindings.Disposables;
using Reactive.Bindings.Extensions;

namespace Specter.ViewModel;

public partial class DeviceViewModel : ObservableObject, IDisposable
{
    private readonly RecordingConfig _recordingConfig;
    private readonly CompositeDisposable _compositeDisposable = new();
    [ObservableProperty] private bool _connected;

    public DeviceViewModel(
        IDevice device,
        RecordingConfig recordingConfig)
    {
        Device = device;
        Device.ObserveProperty(x => x.VolumeLevel)
            .Subscribe(_ => OnPropertyChanged(nameof(VolumeLevel)));
        this.ObserveProperty(x => x.Connected)
            .Skip(1)
            .Subscribe(OnConnected)
            .AddTo(_compositeDisposable);

        Device.InputLevel
            .Subscribe(OnNext);
        _recordingConfig = recordingConfig;
        LiveData = new double[(int)(_recordingConfig.RecordingSpan / _recordingConfig.RefreshRate.Interval)];
        Array.Fill(LiveData, Decibel.Minimum.AsPrimitive());
    }

    public IDevice Device { get; }

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

    public DeviceId Id => Device.Id;
    public DataFlow DataFlow => Device.DataFlow;
    public bool Connectable => DataFlow == DataFlow.Render;

    public string Name
    {
        get => Device.Name;
        set
        {
            Device.Name = value;
            OnPropertyChanged();
        }
    }

    public string SystemName => Device.SystemName;

    /// <summary>
    /// 入力レベル
    /// </summary>
    public string VolumeLevel
    {
        get => (Device.VolumeLevel.AsPrimitive() * 100).ToString("0");
        set
        {
            if(int.TryParse(value, out var intValue))
            {
                if (intValue is >= 0 and <= 100)
                {
                    Device.VolumeLevel = new VolumeLevel(intValue / 100f);
                }
            }
            OnPropertyChanged();
        }
    }

    public bool Measure
    {
        get => Device.Measure;
        set
        {
            Device.Measure = value;
            if (Device.Measure)
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
        Device.StartMonitoring(_recordingConfig.WaveFormat, _recordingConfig.RefreshRate);
    }

    public void PlayLooping(CancellationToken token)
    {
        ((IRenderDevice)Device).PlayLooping(token);
    }

    private RemoteDeviceConnector? _connector;

    private void Connect()
    {
        if (Measure is false)
        {
            Measure = true;
        }
        
        _connector = new("localhost", (IRenderDevice)Device);
        _connector.Connect();
    }

    private void Disconnect()
    {
        _connector?.Dispose();
        _connector = null;
    }

    public void StopMonitoring()
    {
        Device.StopMonitoring();
        Array.Fill(LiveData, Decibel.Minimum.AsPrimitive());
    }

    private void OnNext(Decibel decibel)
    {
        // "scroll" the whole chart to the left
        Array.Copy(LiveData, 1, LiveData, 0, LiveData.Length - 1);
        LiveData[^1] = decibel.AsPrimitive();
    }

    public override string ToString() => Device.Name;

    public void Dispose()
    {
        Device.StopMonitoring();
        Device.Dispose();
    }
}