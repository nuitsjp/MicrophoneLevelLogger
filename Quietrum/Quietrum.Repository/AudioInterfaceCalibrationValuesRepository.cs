using System.Text.Json;

namespace Quietrum.Repository;

/// <summary>
/// AudioInterfaceCalibrationValuesリポジトリー
/// </summary>
public class AudioInterfaceCalibrationValuesRepository : IAudioInterfaceCalibrationValuesRepository
{
    /// <summary>
    /// 設定保存ファイル名。
    /// </summary>
    private const string FileName = "AudioInterfaceCalibrationValues.json";

    /// <summary>
    /// AudioInterfaceCalibrationValuesをロードする。
    /// </summary>
    /// <returns></returns>
    public async Task<AudioInterfaceCalibrationValues> LoadAsync()
    {
        if (!File.Exists(FileName))
        {
            await SaveAsync(new AudioInterfaceCalibrationValues(Array.Empty<MicrophoneCalibrationValue>()));
        }
        await using var stream = new FileStream(FileName, FileMode.Open, FileAccess.Read);
        return (await JsonSerializer.DeserializeAsync<AudioInterfaceCalibrationValues>(stream, JsonEnvironments.Options))!;
    }

    /// <summary>
    /// AudioInterfaceCalibrationValuesをセーブする。
    /// </summary>
    /// <param name="audioInterfaceCalibrationValues"></param>
    /// <returns></returns>
    public async Task SaveAsync(AudioInterfaceCalibrationValues audioInterfaceCalibrationValues)
    {
        await using var stream = new FileStream(FileName, FileMode.Create, FileAccess.Write);
        await JsonSerializer.SerializeAsync(stream, audioInterfaceCalibrationValues, JsonEnvironments.Options);
    }

    /// <summary>
    /// AudioInterfaceCalibrationValuesを削除する。
    /// </summary>
    public void Remove()
    {
        if (File.Exists(FileName))
        {
            File.Delete(FileName);
        }
    }
}