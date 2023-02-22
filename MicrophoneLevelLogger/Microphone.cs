using NAudio.CoreAudioApi;
using NAudio.Wave;
using System.Buffers;
using MMDeviceEnumerator = NAudio.CoreAudioApi.MMDeviceEnumerator;

namespace MicrophoneLevelLogger;

public class Microphone : IMicrophone
{
    public Microphone(MicrophoneId id, string name, string systemName, int deviceNumber, MicrophoneStatus status)
    {
        Id = id;
        DeviceNumber = new DeviceNumber(deviceNumber);
        Name = name;
        SystemName = systemName;
        Status = status;
    }

    public MicrophoneId Id { get; }
    public DeviceNumber DeviceNumber { get; }
    public string Name { get; }
    public string SystemName { get; }
    public MicrophoneStatus Status { get; }

    public VolumeLevel VolumeLevel
    {
        get
        {
            using var mmDevice = GetMmDevice();
            return (VolumeLevel)mmDevice.AudioEndpointVolume.MasterVolumeLevelScalar;
        }
        set
        {
            var mmDevice = GetMmDevice();
            mmDevice.AudioEndpointVolume.MasterVolumeLevelScalar = (float)value;
        }
    }

    private MMDevice GetMmDevice()
    {
        using var enumerator = new MMDeviceEnumerator();
        return enumerator.GetDevice(Id.AsPrimitive());
    }

    public override string ToString() => Name;
}