using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace MicrophoneLevelLogger;

public class AudioInterface : IAudioInterface
{
    private readonly Settings _settings;
    public AudioInterface(Settings settings)
    {
        _settings = settings;
        AllMicrophones = LoadAllMicrophones(settings).ToList();
        Microphones = AllMicrophones
            .Where(x => settings.DisabledMicrophones.NotContains(x.Id))
            .ToList();
    }

    public AudioInterface(Settings settings, params IMicrophone[] microphones)
    {
        _settings = settings;
        Microphones = microphones.ToList();
        AllMicrophones = LoadAllMicrophones(settings).ToList();
    }

    private IEnumerable<Microphone> LoadAllMicrophones(Settings settings)
    {
        using var enumerator = new MMDeviceEnumerator();
        var mmDevices = enumerator
            .EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active)
            .ToArray();
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
                    var microphoneId = new MicrophoneId(mmDevice.ID);
                    var alias = settings.Aliases.SingleOrDefault(x => x.Id == microphoneId)?.Name ?? mmDevice.FriendlyName;
                    yield return new Microphone(
                        microphoneId,
                        alias,
                        mmDevice.FriendlyName, i);
                }
            }
        }
        finally
        {
            foreach (var mmDevice in mmDevices)
            {
                mmDevice.DisposeQuiet();
            }
        }

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
    public IReadOnlyList<IMicrophone> AllMicrophones { get; }

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