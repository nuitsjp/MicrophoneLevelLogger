using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace MicrophoneLevelLogger.Command;

public class AudioInterfaceInputLevels
{
    public IList<MicrophoneInputLevel> Microphones { get; set; } = new List<MicrophoneInputLevel>();

    private const string FileName = "AudioInterfaceInputLevels.json";

    private static JsonSerializerOptions Options => new()
    {
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public void Update(MicrophoneInputLevel microphoneInputLevel)
    {
        var old = Microphones.SingleOrDefault(x => x.Name == microphoneInputLevel.Name);
        if (old is not null)
        {
            Microphones.Remove(old);
        }

        Microphones.Add(microphoneInputLevel);
    }

    public static async Task<AudioInterfaceInputLevels> LoadAsync()
    {
        if (!File.Exists(FileName))
        {
            await SaveAsync(new AudioInterfaceInputLevels());
        }
        await using var stream = new FileStream(FileName, FileMode.Open, FileAccess.Read);
        return (await JsonSerializer.DeserializeAsync<AudioInterfaceInputLevels>(stream, Options))!;
    }

    public static void Remove()
    {
        if (File.Exists(FileName))
        {
            File.Delete(FileName);
        }
    }

    public static async Task SaveAsync(AudioInterfaceInputLevels audioInterfaceInputLevels)
    {
        await using var stream = new FileStream(FileName, FileMode.Create, FileAccess.Write);
        await JsonSerializer.SerializeAsync(stream, audioInterfaceInputLevels, Options);

    }
}