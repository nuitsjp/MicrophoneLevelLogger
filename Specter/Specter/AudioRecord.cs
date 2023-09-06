namespace Specter.Business;

public record AudioRecord(
    string TargetDeviceName,
    Direction Direction,
    DateTime StartTime,
    DateTime StopTime,
    DeviceRecord[] DeviceRecords);