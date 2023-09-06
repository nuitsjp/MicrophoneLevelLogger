namespace Specter.Business;

public interface IDeviceRecorder
{
    void Start();
    DeviceRecord Stop();
}