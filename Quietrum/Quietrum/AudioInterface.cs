using System.ComponentModel;
using System.Management;
using System.Reactive.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using NAudio.CoreAudioApi;

namespace Quietrum;

/// <summary>
/// マイク・スピーカーなどを統合したオーディオ関連のインターフェース
/// </summary>
public partial class AudioInterface : ObservableObject, IAudioInterface
{
    private readonly ISettingsRepository _settingsRepository;

    private readonly IRemoteDeviceServer _remoteDeviceServer;

    private readonly LocalDeviceInterface _localDeviceInterface;

    /// <summary>
    /// 
    /// </summary>
    [ObservableProperty] private IReadOnlyList<IDevice> _devices = new List<IDevice>();

    private readonly List<RemoteDevice> _remoteDevices = new();

    private Settings _settings;

    /// <summary>
    /// すべてのマイクを扱うオーディオ インターフェースを作成する。
    /// </summary>
    /// <param name="settingsRepository"></param>
    /// <param name="remoteDeviceServer"></param>
    /// <param name="localDeviceInterface"></param>
    internal AudioInterface(
        ISettingsRepository settingsRepository, 
        IRemoteDeviceServer remoteDeviceServer, 
        LocalDeviceInterface localDeviceInterface)
    {
        _settingsRepository = settingsRepository;
        _remoteDeviceServer = remoteDeviceServer;
        _localDeviceInterface = localDeviceInterface;
        _localDeviceInterface.ConnectedDevice += OnConnectedDevice;
        _localDeviceInterface.DisconnectedDevice += OnDisconnectedDevice;
        _settings = default!;
        _remoteDeviceServer.RemoteDevicesChanged += (_, _) => ReloadDevices();
    }

    private void OnConnectedDevice(object? sender, DeviceEventArgs e)
    {
        var devices = Devices.ToList();
        devices.Add(e.Device);
        Devices = devices;
    }

    private void OnDisconnectedDevice(object? sender, DeviceEventArgs e)
    {
        var devices = Devices.ToList();
        devices.Remove(e.Device);
        Devices = devices;
    }

    public async Task ActivateAsync()
    {
        _settings = await _settingsRepository.LoadAsync();
        _remoteDeviceServer.Activate();
        await _localDeviceInterface.ActivateAsync();
        ReloadDevices();
    }

    /// <summary>
    /// すべてのマイクをリロードする。
    /// </summary>
    /// <returns></returns>
    private void ReloadDevices()
    {
        Devices = _localDeviceInterface
            .Devices
            .ToList();
    }

    /// <summary>
    /// 現在有効なスピーカーを取得する。
    /// </summary>
    /// <returns></returns>
    public async Task<ISpeaker> GetSpeakerAsync()
    {
        var settings = await _settingsRepository.LoadAsync();
        using var enumerator = new MMDeviceEnumerator();
        // 明示的に指定されていればそれを、指定されていない場合、OSの出力先へ出力する。
        if (settings.SelectedSpeakerId is null)
        {
            using var mmDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            return new Speaker(new SpeakerId(mmDevice.ID), mmDevice.FriendlyName);
        }
        else
        {
            try
            {
                using var mmDevice = enumerator.GetDevice(settings.SelectedSpeakerId?.AsPrimitive());
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
                        null,
                        settings.MicrophoneConfigs));
                using var mmDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
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
        using var enumerator = new MMDeviceEnumerator();
        var mmDevices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
        foreach (var mmDevice in mmDevices)
        {
            using (mmDevice)
            {
                yield return new Speaker(new SpeakerId(mmDevice.ID), mmDevice.FriendlyName);
            }
        }
    }
    
    public void Dispose()
    {
        Devices.Dispose();
    }
}