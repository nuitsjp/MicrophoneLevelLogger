using NAudio.CoreAudioApi;

namespace Specter.Business;

public class CaptureDevice : Device, ICaptureDevice
{
    public CaptureDevice(
        DeviceId id,
        string name, 
        string systemName, 
        bool measure, 
        MMDevice mmDevice) : base(id, name, systemName, measure, mmDevice)
    {
    }

    public override DataFlow DataFlow => DataFlow.Capture;
}