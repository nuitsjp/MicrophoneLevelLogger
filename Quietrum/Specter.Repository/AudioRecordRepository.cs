using System.Text.Json;
using Specter.Business;

namespace Specter.Repository;

public class AudioRecordRepository : IAudioRecordRepository
{
    private static readonly string RootDirectory = "Record";
    
    public async Task SaveAsync(AudioRecord audioRecord)
    {
        var targetDevice = audioRecord.DeviceRecords.Single(x => x.Id == audioRecord.TargetDeviceId);
        var filePath = Path.Combine(
            RootDirectory,
            $"{audioRecord.StartTime:yyyy.MM.dd-HH.mm.ss}_{targetDevice.Name}_{audioRecord.Direction}",
            "AudioRecord.json");
        
        var directoryName = Path.GetDirectoryName(filePath)!;
        if (!Directory.Exists(directoryName))
            Directory.CreateDirectory(directoryName);
        
        await using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        await JsonSerializer.SerializeAsync(stream, audioRecord, JsonEnvironments.Options);
    }

    public Task<IEnumerable<AudioRecord>> LoadAsync()
    {
        throw new NotImplementedException();
    }
}