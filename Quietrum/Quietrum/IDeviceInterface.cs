namespace Quietrum;

public interface IDeviceInterface<out TDevice> where TDevice : IDevice
{
    public event EventHandler<DeviceEventArgs>? ConnectedDevice;
    public event EventHandler<DeviceEventArgs>? DisconnectedDevice;
    
    public IReadOnlyList<TDevice> Devices { get; }

    public Task ActivateAsync();
}