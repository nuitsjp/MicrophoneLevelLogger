using System.ComponentModel;
using System.Management;
using NAudio.CoreAudioApi;

namespace Quietrum;

public class LocalDeviceInterface : IDeviceInterface<IDevice>
{
    private readonly ManagementEventWatcher _watcher = new(
        new WqlEventQuery("__InstanceOperationEvent")
        {
            WithinInterval = TimeSpan.FromSeconds(3),
            Condition = "TargetInstance ISA 'Win32_SoundDevice'"
        });

    private readonly ISettingsRepository _settingsRepository;
    private readonly List<IDevice> _devices = new();
    private Settings _settings = default!;

    public LocalDeviceInterface(ISettingsRepository settingsRepository)
    {
        _settingsRepository = settingsRepository;
    }

    public event EventHandler<DeviceEventArgs>? ConnectedDevice;
    public event EventHandler<DeviceEventArgs>? DisconnectedDevice;

    public IReadOnlyList<IDevice> Devices => _devices;

    public async Task ActivateAsync()
    {
        _settings = await _settingsRepository.LoadAsync();
        _watcher.EventArrived += WatcherEventArrived;
        _watcher.Start();
        await LoadDevicesAsync();
    }
    
    private async void WatcherEventArrived(object sender, EventArrivedEventArgs e)
    {
        if (e.NewEvent["TargetInstance"] is not ManagementBaseObject) return;
        
        var eventType = e.NewEvent.ClassPath.ClassName;
        switch (eventType)
        {
            case "__InstanceCreationEvent":
            case "__InstanceDeletionEvent":
                await LoadDevicesAsync();
                break;
        }
    }
    
    /// <summary>
    /// すべてのマイクをロードする。
    /// </summary>
    /// <returns></returns>
    private async Task LoadDevicesAsync()
    {
        using var enumerator = new MMDeviceEnumerator();
        var mmDevices = enumerator
            .EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active)
            .ToList();

        // 新しく接続されたデバイスの確認
        var connectedDevices = mmDevices
            .Where(mmDevice => _devices.NotContains(device => device.Id.AsPrimitive() == mmDevice.ID));
        foreach (var mmDevice in connectedDevices)
        {
            var device = await ResolveDeviceAsync(mmDevice);
            _devices.Add(device);
            ConnectedDevice?.Invoke(this, new DeviceEventArgs(device));
        }
        
        // 切断されたデバイスの確認
        var disconnectedDevices = _devices
            .Where(device => mmDevices.NotContains(mmDevice => device.Id.AsPrimitive() == mmDevice.ID))
            .ToList(); // _devicesから作成しているので、いったん別のListにしないとRemove時にエラーとなるので詰め替えておく
        foreach (var device in disconnectedDevices)
        {
            _devices.Remove(device);
            device.Dispose();
            DisconnectedDevice?.Invoke(this, new DeviceEventArgs(device));
        }
    }
    
    private async void MicrophoneOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not IDevice microphone) return;
        if(e.PropertyName is not (
           nameof(IDevice.Name) 
           or nameof(IDevice.Measure))) return;
        
        var config = _settings.GetMicrophoneConfig(microphone.Id);
        config.Name = microphone.Name;
        config.Measure = microphone.Measure;
        await _settingsRepository.SaveAsync(_settings);
    }

    private async Task<IDevice> ResolveDeviceAsync(MMDevice mmDevice)
    {
        var microphoneId = new DeviceId(mmDevice.ID);
        // 新たに接続されたマイクだった場合
        if (_settings.TryGetMicrophoneConfig(microphoneId, out var microphoneConfig) is false)
        {
            microphoneConfig = new MicrophoneConfig(microphoneId, mmDevice.FriendlyName, true);
            _settings.AddMicrophoneConfig(microphoneConfig);
            await _settingsRepository.SaveAsync(_settings);
        }

        var device = new Device(
            microphoneId,
            microphoneConfig.Name,
            mmDevice.FriendlyName, 
            microphoneConfig.Measure,
            mmDevice);
        device.PropertyChanged += MicrophoneOnPropertyChanged;
        return device;
    }
}