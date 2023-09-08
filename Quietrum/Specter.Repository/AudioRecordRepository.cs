using System.Text.Json;

namespace Specter.Repository;

public class AudioRecordRepository : IAudioRecordRepository
{
    private static readonly string RootDirectory = "Record";
    private static readonly string AudioRecordFile = "AudioRecord.json";

    public static string GetAudioRecordPath(AudioRecord audioRecord)
    {
        var targetDevice = audioRecord.DeviceRecords.Single(x => x.Id == audioRecord.TargetDeviceId);
        return Path.Combine(
            RootDirectory,
            $"{audioRecord.StartTime:yyyy.MM.dd-HH.mm.ss}_{targetDevice.Name}_{audioRecord.Direction}");
    }
    
    public async Task SaveAsync(AudioRecord audioRecord)
    {
        var filePath = Path.Combine(
            GetAudioRecordPath(audioRecord),
            AudioRecordFile);
        
        var directoryName = Path.GetDirectoryName(filePath)!;
        if (!Directory.Exists(directoryName))
            Directory.CreateDirectory(directoryName);
        
        await using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        await JsonSerializer.SerializeAsync(stream, audioRecord, JsonEnvironments.Options);
    }

    public async Task<IEnumerable<AudioRecord>> LoadAsync()
    {
        var directories = Directory.GetDirectories(RootDirectory);

        List<AudioRecord> records = new();
        foreach (var directory in directories)
        {
            var file = Path.Combine(directory, AudioRecordFile);
            if (File.Exists(file) is false)
                continue;
            
            await using var stream = new FileStream(file, FileMode.Open, FileAccess.Read);
            records.Add((await JsonSerializer.DeserializeAsync<AudioRecord>(stream, JsonEnvironments.Options))!);
        }

        return records;
    }
}