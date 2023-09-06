namespace Specter.Business;

/// <summary>
/// 
/// </summary>
/// <param name="Id">ターゲットデバイスID</param>
/// <param name="Name"></param>
/// <param name="SystemName"></param>
/// <param name="WaveFile"></param>
/// <param name="InputLevelFile"></param>
/// <param name="Min"></param>
/// <param name="Avg"></param>
/// <param name="Max"></param>
/// <param name="Minus30db"></param>
/// <param name="Minus40db"></param>
/// <param name="Minus50db"></param>
public record DeviceRecord(
    DeviceId Id,
    string Name,
    string SystemName,
    FileInfo WaveFile,
    FileInfo InputLevelFile,
    Decibel Min,
    Decibel Avg,
    Decibel Max,
    double Minus30db,
    double Minus40db,
    double Minus50db);