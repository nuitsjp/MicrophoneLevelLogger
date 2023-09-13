using NAudio.CoreAudioApi;

namespace Specter;

public class CaptureDevice : Device, ICaptureDevice
{
    public CaptureDevice(
        DeviceId id,
        string name, 
        string systemName, 
        bool measure, 
        MMDevice mmDevice,
        IFastFourierTransformSettings settings) 
        : base(
            id, 
            name, 
            systemName, 
            measure, 
            mmDevice,
            settings)
    {
    }

    public override DataFlow DataFlow => DataFlow.Capture;
}