using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace MicrophoneLevelLogger.Domain;

public class AudioInterfaceCalibrationValues
{
    public IList<MicrophoneCalibrationValue> Microphones { get; set; } = new List<MicrophoneCalibrationValue>();

    private const string FileName = "AudioInterfaceCalibrationValues.json";

    private static JsonSerializerOptions Options => new()
    {
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public void Update(MicrophoneCalibrationValue microphoneCalibrationValue)
    {
        var old = Microphones.SingleOrDefault(x => x.Name == microphoneCalibrationValue.Name);
        if (old is not null)
        {
            Microphones.Remove(old);
        }

        Microphones.Add(microphoneCalibrationValue);
    }

    public static async Task<AudioInterfaceCalibrationValues> LoadAsync()
    {
        if (!File.Exists(FileName))
        {
            await SaveAsync(new AudioInterfaceCalibrationValues());
        }
        await using var stream = new FileStream(FileName, FileMode.Open, FileAccess.Read);
        return (await JsonSerializer.DeserializeAsync<AudioInterfaceCalibrationValues>(stream, Options))!;
    }

    public static void Remove()
    {
        if (File.Exists(FileName))
        {
            File.Delete(FileName);
        }
    }

    public static async Task SaveAsync(AudioInterfaceCalibrationValues audioInterfaceCalibrationValues)
    {
        await using var stream = new FileStream(FileName, FileMode.Create, FileAccess.Write);
        await JsonSerializer.SerializeAsync(stream, audioInterfaceCalibrationValues, Options);

    }

}