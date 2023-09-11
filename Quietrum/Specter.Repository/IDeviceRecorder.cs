namespace Specter;

public interface IDeviceRecorder
{
    void Start();
    DeviceRecord Stop();
}