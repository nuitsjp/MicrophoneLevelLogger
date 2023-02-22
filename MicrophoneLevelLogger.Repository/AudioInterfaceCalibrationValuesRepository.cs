using System.Text.Json;

namespace MicrophoneLevelLogger.Repository;

public class AudioInterfaceCalibrationValuesRepository : IAudioInterfaceCalibrationValuesRepository
{
    private const string FileName = "AudioInterfaceCalibrationValues.json";

    public async Task<AudioInterfaceCalibrationValues> LoadAsync()
    {
        if (!File.Exists(FileName))
        {
            await SaveAsync(new AudioInterfaceCalibrationValues());
        }
        await using var stream = new FileStream(FileName, FileMode.Open, FileAccess.Read);
        return (await JsonSerializer.DeserializeAsync<AudioInterfaceCalibrationValues>(stream, JsonEnvironments.Options))!;
    }

    public async Task SaveAsync(AudioInterfaceCalibrationValues audioInterfaceCalibrationValues)
    {
        await using var stream = new FileStream(FileName, FileMode.Create, FileAccess.Write);
        await JsonSerializer.SerializeAsync(stream, audioInterfaceCalibrationValues, JsonEnvironments.Options);
    }

    public void Remove()
    {
        if (File.Exists(FileName))
        {
            File.Delete(FileName);
        }
    }
}