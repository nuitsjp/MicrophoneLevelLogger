namespace Specter.Business;

public record DeviceRecord(
    DirectoryInfo DirectoryInfo,
    string DeviceName,
    Decibel[] InputLevels);