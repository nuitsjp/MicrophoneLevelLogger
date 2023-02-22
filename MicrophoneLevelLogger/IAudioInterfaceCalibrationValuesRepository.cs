namespace MicrophoneLevelLogger;

/// <summary>
/// AudioInterfaceCalibrationValuesのリポジトリー
/// </summary>
public interface IAudioInterfaceCalibrationValuesRepository
{
    /// <summary>
    /// AudioInterfaceCalibrationValuesをロードする。
    /// </summary>
    /// <returns></returns>
    Task<AudioInterfaceCalibrationValues> LoadAsync();

    /// <summary>
    /// AudioInterfaceCalibrationValuesをセーブする。
    /// </summary>
    /// <param name="audioInterfaceCalibrationValues"></param>
    /// <returns></returns>
    Task SaveAsync(AudioInterfaceCalibrationValues audioInterfaceCalibrationValues);

    /// <summary>
    /// AudioInterfaceCalibrationValuesを削除する。
    /// </summary>
    void Remove();
}