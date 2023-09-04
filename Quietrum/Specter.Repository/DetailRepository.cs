using Specter.Business;

namespace Specter.Repository;

/// <summary>
/// マイク別のサンプリング間隔ごとの最大音量を記録するライター
/// </summary>
public class DetailRepository : IDetailRepository
{
    /// <summary>
    /// 出力先
    /// </summary>
    private readonly StreamWriter _writer;

    /// <summary>
    /// インスタンスを生成する。
    /// </summary>
    /// <param name="writer"></param>
    public DetailRepository(StreamWriter writer)
    {
        _writer = writer;
    }

    /// <summary>
    /// ヘッダーを記録する。
    /// </summary>
    /// <param name="recorders"></param>
    /// <returns></returns>
    public async Task WriteHeaderAsync(IEnumerable<IMicrophoneRecorder> recorders)
    {
        await _writer.WriteAsync("時刻");
        foreach (var microphoneLogger in recorders)
        {
            await _writer.WriteAsync(",");
            await _writer.WriteAsync(microphoneLogger.Device.Name);
        }

        await _writer.WriteLineAsync();
    }

    /// <summary>
    /// サンプリング結果を記録する。
    /// </summary>
    /// <param name="recorders"></param>
    /// <returns></returns>
    public async Task WriteRecordAsync(IEnumerable<IMicrophoneRecorder> recorders)
    {
        await _writer.WriteAsync($"{DateTime.Now:yyyy/MM/dd hh:mm:ss.fff}");
        foreach (var microphoneLogger in recorders)
        {
            await _writer.WriteAsync(",");
            _writer.Write(microphoneLogger.Max);
        }

        await _writer.WriteLineAsync();
    }

    /// <summary>
    /// リソースを開放する。
    /// </summary>
    public void Dispose()
    {
        _writer.Flush();
        _writer.Dispose();
        GC.SuppressFinalize(this);
    }
}