using NAudio.Wave;

namespace Specter.Business;

public class WaveRecorder
{
    private readonly IDevice _device;
    private readonly WaveFileWriter _fileWriter;
    private IDisposable? _disposable;

    public WaveRecorder(
        IDevice device,
        FileInfo fileInfo,
        WaveFormat waveFormat)
    {
        _device = device;
        _fileWriter = new WaveFileWriter(fileInfo.FullName, waveFormat);
    }

    public void StartRecording()
    {
        _disposable = _device
            .WaveInput
            .Subscribe(
                onNext: e => _fileWriter.Write(e.Buffer, 0, e.BytesRecorded),
                onCompleted: OnCompleted);
    }

    public void StopRecording()
    {
        if(_disposable is null) return;
        
        OnCompleted();
    }

    private void OnCompleted()
    {
        _disposable?.Dispose();
        _fileWriter.Flush();
        _fileWriter.Close();
        _fileWriter.Dispose();
        _disposable = null;
    }
}