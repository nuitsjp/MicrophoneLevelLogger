using NAudio.Wave;

namespace Quietrum;

public class WaveRecorder
{
    private readonly IObservable<byte[]> _observable;
    private readonly WaveFileWriter _fileWriter;
    private IDisposable? _disposable;

    public WaveRecorder(
        FileInfo fileInfo,
        WaveFormat waveFormat, 
        IObservable<byte[]> observable)
    {
        _observable = observable;
        _fileWriter = new WaveFileWriter(fileInfo.FullName, waveFormat);
    }

    public void StartRecording()
    {
        _disposable = _observable.Subscribe(
            onNext: bytes => _fileWriter.Write(bytes, 0, bytes.Length),
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