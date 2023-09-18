namespace Specter;

public record AudioRecord(
    DeviceId TargetDeviceId,
    Direction Direction,
    BuzzState BuzzState,
    DateTime StartTime,
    DateTime StopTime,
    IReadOnlyList<DeviceRecord> DeviceRecords);