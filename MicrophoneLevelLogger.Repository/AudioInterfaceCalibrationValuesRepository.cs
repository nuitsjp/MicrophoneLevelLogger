using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace MicrophoneLevelLogger.Repository;

public class AudioInterfaceCalibrationValuesRepository : IAudioInterfaceCalibrationValuesRepository
{
    private const string FileName = "AudioInterfaceCalibrationValues.json";

    private static JsonSerializerOptions Options => new()
    {
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };


    public async Task<AudioInterfaceCalibrationValues> LoadAsync()
    {
        if (!File.Exists(FileName))
        {
            await SaveAsync(new AudioInterfaceCalibrationValues());
        }
        await using var stream = new FileStream(FileName, FileMode.Open, FileAccess.Read);
        return (await JsonSerializer.DeserializeAsync<AudioInterfaceCalibrationValues>(stream, Options))!;
    }

    public async Task SaveAsync(AudioInterfaceCalibrationValues audioInterfaceCalibrationValues)
    {
        await using var stream = new FileStream(FileName, FileMode.Create, FileAccess.Write);
        await JsonSerializer.SerializeAsync(stream, audioInterfaceCalibrationValues, Options);
    }

    public void Remove()
    {
        if (File.Exists(FileName))
        {
            File.Delete(FileName);
        }
    }
}