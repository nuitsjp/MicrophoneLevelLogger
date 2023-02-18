using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace MicrophoneLevelLogger.Repository;

public class AudioInterfaceInputLevelsRepository : IAudioInterfaceInputLevelsRepository
{
    private const string FileName = "AudioInterfaceInputLevels.json";

    public async Task<AudioInterfaceInputLevels> LoadAsync()
    {
        if (!File.Exists(FileName))
        {
            await SaveAsync(new AudioInterfaceInputLevels());
        }
        await using var stream = new FileStream(FileName, FileMode.Open, FileAccess.Read);
        return (await JsonSerializer.DeserializeAsync<AudioInterfaceInputLevels>(stream, JsonEnvironments.Options))!;
    }

    public async Task SaveAsync(AudioInterfaceInputLevels audioInterfaceInputLevels)
    {
        await using var stream = new FileStream(FileName, FileMode.Create, FileAccess.Write);
        await JsonSerializer.SerializeAsync(stream, audioInterfaceInputLevels, JsonEnvironments.Options);
    }

    public void Remove()
    {
        if (File.Exists(FileName))
        {
            File.Delete(FileName);
        }
    }
}