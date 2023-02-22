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

    public async Task<IEnumerable<RecordSummary>> LoadAsync()
    {
        var fileInfos = JsonEnvironments.RecordDirectory.GetFiles(FileName, SearchOption.AllDirectories);
        List<RecordSummary> summaries = new();
        foreach (var fileInfo in fileInfos)
        {
            await using var stream = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read);
            summaries.Add((await JsonSerializer.DeserializeAsync<RecordSummary>(stream, JsonEnvironments.Options))!);
        }

        return summaries;
    }
}