namespace Specter;

public record AudioRecord(
    DeviceId TargetDeviceId,
    RecordingMethod RecordingMethod,
    DateTime StartTime,
    DateTime StopTime,
    IReadOnlyList<DeviceRecord> DeviceRecords);