using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace MicrophoneLevelLogger;

/// <summary>
/// マイク・スピーカーなどを統合したオーディオ関連のインターフェース
/// </summary>
public class AudioInterface : IAudioInterface
{
    private readonly ISettingsRepository _settingsRepository;

    /// <summary>
    /// 対象マイク
    /// </summary>
    private readonly IReadOnlyList<IMicrophone> _microphones;

    /// <summary>
    /// すべてのマイクを扱うオーディオ インターフェースを作成する。
    /// </summary>
    /// <param name="settingsRepository"></param>
    public AudioInterface(ISettingsRepository settingsRepository)
    {
        _settingsRepository = settingsRepository;
        _microphones = LoadMicrophones(_settingsRepository.Load()).ToList();
    }

    /// <summary>
    /// 任意のマイクを扱うオーディオ インターフェースを作成する。
    /// </summary>
    /// <param name="settingsRepository"></param>
    /// <param name="microphones"></param>
    public AudioInterface(ISettingsRepository settingsRepository, params IMicrophone[] microphones)
    {
        _settingsRepository = settingsRepository;
        _microphones = microphones;
    }

    /// <summary>
    /// すべてのマイクをロードする。
    /// </summary>
    /// <param name="settings"></param>
    /// <returns></returns>
    private IEnumerable<Microphone> LoadMicrophones(Settings settings)
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

    /// <summary>
    /// 状態が合致するマイクを取得する。
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    public IEnumerable<IMicrophone> GetMicrophones(MicrophoneStatus status = MicrophoneStatus.Enable) =>
        _microphones.Where(x => status.HasFlag(x.Status));

    /// <summary>
    /// 現在有効なスピーカーを取得する。
    /// </summary>
    /// <returns></returns>
    public async Task<ISpeaker> GetSpeakerAsync()
    {
        var settings = await _settingsRepository.LoadAsync();
        using var emurator = new MMDeviceEnumerator();
        // 明示的に指定されていればそれを、指定されていない場合、OSの出力先へ出力する。
        if (settings.SelectedSpeakerId is null)
        {
            using var mmDevice = emurator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            return new Speaker(new SpeakerId(mmDevice.ID), mmDevice.FriendlyName);
        }
        else
        {
            try
            {
                using var mmDevice = emurator.GetDevice(settings.SelectedSpeakerId?.AsPrimitive());
                return new Speaker(new SpeakerId(mmDevice.ID), mmDevice.FriendlyName);
            }
            catch
            {
                // スピーカーを外したりすると、デバイスが見つからなくなるのでその場合は削除してデフォルトを返す。
                await _settingsRepository.SaveAsync(
                    new(
                        settings.MediaPlayerHost,
                        settings.RecorderHost,
                        settings.RecordingSpan,
                        settings.IsEnableRemotePlaying,
                        settings.IsEnableRemoteRecording,
                        settings.Aliases,
                        settings.DisabledMicrophones,
                        null));
                using var mmDevice = emurator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                return new Speaker(new SpeakerId(mmDevice.ID), mmDevice.FriendlyName);
            }
        }
    }

    /// <summary>
    /// すべてのスピーカーを取得する。
    /// </summary>
    /// <returns></returns>
    public IEnumerable<ISpeaker> GetSpeakers()
    {
        using var emurator = new MMDeviceEnumerator();
        var mmDevices = emurator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
        foreach (var mmDevice in mmDevices)
            using (mmDevice)
            {
                yield return new Speaker(new SpeakerId(mmDevice.ID), mmDevice.FriendlyName);
            }
    }
}