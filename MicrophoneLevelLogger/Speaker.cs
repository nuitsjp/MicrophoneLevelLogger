using NAudio.CoreAudioApi;

namespace MicrophoneLevelLogger;

public class Speaker : ISpeaker
{
    public Speaker(SpeakerId id, string name)
    {
        Id = id;
        Name = name;
    }

    public SpeakerId Id { get; }
    public string Name { get; }

    public VolumeLevel VolumeLevel
    {
        get
        {
            using var deviceEnumerator = new MMDeviceEnumerator();
            using var device = deviceEnumerator.GetDevice(Id.AsPrimitive());
            return new VolumeLevel(device.AudioEndpointVolume.MasterVolumeLevelScalar);
        }
        set
        {
            using var deviceEnumerator = new MMDeviceEnumerator();
            using var device = deviceEnumerator.GetDevice(Id.AsPrimitive());
            device.AudioEndpointVolume.MasterVolumeLevelScalar = value.AsPrimitive();
        }

    }
}