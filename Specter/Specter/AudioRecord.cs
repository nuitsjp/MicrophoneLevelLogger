namespace Specter;

public record AudioRecord(
    DeviceId TargetDeviceId,
    Direction Direction,
    DateTime StartTime,
    DateTime StopTime,
    IReadOnlyList<DeviceRecord> DeviceRecords);