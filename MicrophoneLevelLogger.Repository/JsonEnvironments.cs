using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace MicrophoneLevelLogger.Repository;

public static class JsonEnvironments
{
    public static readonly DirectoryInfo RecordDirectory = new("Record");

    public static JsonSerializerOptions Options => new()
    {
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

}