using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace MicrophoneLevelLogger;

public class AudioInterface : IAudioInterface
{
    private readonly ISettingsRepository _settingsRepository;
    private readonly IReadOnlyList<IMicrophone> _microphones;
    public AudioInterface(ISettingsRepository settingsRepository)
    {
        _settingsRepository = settingsRepository;
        _microphones = LoadAllMicrophones(_settingsRepository.Load()).ToList();
    }

    public AudioInterface(ISettingsRepository settingsRepository, params IMicrophone[] microphones)
    {
        _settingsRepository = settingsRepository;
        _microphones = microphones;
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
                        mmDevice.FriendlyName, 
                        i,
                        settings.DisabledMicrophones.NotContains(microphoneId) 
                            ? MicrophoneStatus.Enable 
                            : MicrophoneStatus.Disable);
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
        foreach (var microphone in GetMicrophones())
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

    public IEnumerable<IMicrophone> GetMicrophones(MicrophoneStatus status = MicrophoneStatus.Enable) =>
        _microphones.Where(x => status.HasFlag(x.Status));

    public IMediaPlayer GetMediaPlayer(bool isRemotePlay)
    {
        if (isRemotePlay is false)
        {
            return new RemoteMediaPlayer(_settingsRepository);
        }
        throw new NotImplementedException();
    }

    public void ActivateMicrophones()
    {
        var tasks = GetMicrophones()
            .Select(x => x.ActivateAsync());
        Task.WaitAll(tasks.ToArray());
    }
}