namespace Specter;

public record AudioRecord(
    DeviceId TargetDeviceId,
    Direction Direction,
    BuzzState BuzzState,
    VoiceState VoiceState,
    DateTime StartTime,
    DateTime StopTime,
    IReadOnlyList<DeviceRecord> DeviceRecords);