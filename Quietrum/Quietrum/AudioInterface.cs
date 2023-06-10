using System.Collections.ObjectModel;
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
    // [ObservableProperty] private IReadOnlyList<IDevice> _devices = new List<IDevice>();

    private readonly ObservableCollection<IDevice> _devices = new();

    public ReadOnlyObservableCollection<IDevice> Devices { get; }

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
        Devices = new ReadOnlyObservableCollection<IDevice>(_devices);
    }

    private void OnConnectedDevice(object? sender, DeviceEventArgs e)
    {
        _devices.Add(e.Device);
    }

    private void OnDisconnectedDevice(object? sender, DeviceEventArgs e)
    {
        _devices.Remove(e.Device);
    }

    public async Task ActivateAsync()
    {
        //_remoteDeviceServer.Activate();
        await _localDeviceInterface.ActivateAsync();
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