namespace Specter;

/// <summary>
/// MicrophoneLevelLoggerの各種設定
/// </summary>
public record Settings(
    string RecorderHost,
    TimeSpan RecordingSpan,
    DeviceId? RecordDeviceId,
    DeviceId? PlaybackDeviceId, 
    IReadOnlyList<DeviceConfig> DeviceConfigs)
{
    public bool TryGetMicrophoneConfig(DeviceId id, out DeviceConfig deviceConfig)
    {
        var config = DeviceConfigs.SingleOrDefault(x => x.Id == id);
        deviceConfig = config!;
        return config is not null;
    }

    public DeviceConfig GetMicrophoneConfig(DeviceId id)
    {
        return DeviceConfigs.Single(x => x.Id == id);
    }
}