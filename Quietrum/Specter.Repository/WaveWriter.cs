using NAudio.Wave;
using Specter.Business;

namespace Specter.Repository;

public class WaveWriter : IDisposable
{
    private readonly WaveFileWriter _waveFileWriter;

    public WaveWriter(string directory, IDevice device, WaveFormat waveFormat)
    {
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
        
        var file = Path.Combine(directory, $"{device.Name}.wav");
        _waveFileWriter = new WaveFileWriter(file, waveFormat);
    }

    public void Write(byte[] bytes, int offset, int count)
    {
        _waveFileWriter.Write(bytes, offset, count);
    }

    public void Dispose()
    {
        _waveFileWriter.Flush();
        _waveFileWriter.Dispose();
    }
}