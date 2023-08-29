using System.Text.Json;
using Specter.Business;

namespace Quietrum.Repository;

public class WaveRecordIndexRepository : IWaveRecordIndexRepository
{
    public async Task SaveAsync(FileInfo fileInfo, WaveRecordIndex waveRecordIndex)
    {
        await using var stream = new FileStream(fileInfo.FullName, FileMode.Create, FileAccess.Write);
        await JsonSerializer.SerializeAsync(stream, waveRecordIndex, JsonEnvironments.Options);
    }
}