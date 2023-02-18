using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace MicrophoneLevelLogger.Repository;

public class RecordSummaryRepository : IRecordSummaryRepository
{
    private const string FileName = "summary.json";

    public async Task SaveAsync(RecordSummary recordSummary, DirectoryInfo directory)
    {

        await using var stream = new FileStream(Path.Combine(directory.FullName, FileName), FileMode.Create, FileAccess.Write);
        await JsonSerializer.SerializeAsync(stream, recordSummary, JsonEnvironments.Options);
    }

    public async IAsyncEnumerable<RecordSummary> LoadAsync()
    {
        var fileInfos = JsonEnvironments.RootDirectory.GetFiles(FileName, SearchOption.AllDirectories);
        foreach (var fileInfo in fileInfos)
        {
            await using var stream = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read);
            yield return (await JsonSerializer.DeserializeAsync<RecordSummary>(stream, JsonEnvironments.Options))!;
        }
    }
}