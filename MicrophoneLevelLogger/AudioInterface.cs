using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace MicrophoneLevelLogger;

public class AudioInterface : IAudioInterface
{

    public AudioInterface(Settings settings)
    {
        using var enumerator = new MMDeviceEnumerator();
        var mmDevices = enumerator
                .EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active)
                .ToArray();
        List<IMicrophone> devices = new();
        try
        {
            for (int i = 0; i < WaveIn.DeviceCount; i++)
            {
                var capability = WaveIn.GetCapabilities(i);
                var name = capability.ProductName;
                // 名称が長いとWaveIn側の名前は途中までしか取得できないため、前方一致で判定する
                var mmDevice = mmDevices.SingleOrDefault(x => x.FriendlyName.StartsWith(name));
                if (mmDevice is not null)
                {
                    var alias = settings.Aliases.SingleOrDefault(x => x.Id == mmDevice.ID)?.Name ?? mmDevice.FriendlyName;
                    devices.Add(
                        new Microphone(
                            mmDevice.ID,
                            alias, 
                            mmDevice.FriendlyName, i));
                }

            }

            Microphones = devices;
        }
        finally
        {
            foreach (var mmDevice in mmDevices)
            {
                mmDevice.DisposeQuiet();
            }
        }
    }

    public AudioInterface(params IMicrophone[] microphones)
    {
        Microphones = microphones.ToList();
    }

    public void Dispose()
    {
        foreach (var microphone in Microphones)
        {
            try
            {
                microphone.Dispose();
            }
            catch
            {
                // ignore
            }
        }
        GC.SuppressFinalize(this);
    }

    public VolumeLevel DefaultOutputLevel
    {
        get
        {
            using var deviceEnumerator = new MMDeviceEnumerator();
            using var device = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            return new VolumeLevel(device.AudioEndpointVolume.MasterVolumeLevelScalar);
        }
        set
        {
            using var deviceEnumerator = new MMDeviceEnumerator();
            using var device = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            device.AudioEndpointVolume.MasterVolumeLevelScalar = value.AsPrimitive();
        }
    }
    public IReadOnlyList<IMicrophone> Microphones { get; }
    public void ActivateMicrophones()
    {
        var tasks = Microphones
            .Select(x => x.ActivateAsync());
        Task.WaitAll(tasks.ToArray());
    }

    public void DeactivateMicrophones()
    {
        foreach (var microphone in Microphones)
        {
            microphone.Deactivate();
        }
    }
}