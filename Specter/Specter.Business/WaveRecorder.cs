using NAudio.Wave;

namespace Specter.Business;

public class WaveRecorder
{
    private readonly IObservable<WaveInEventArgs> _observable;
    private readonly WaveFileWriter _fileWriter;
    private IDisposable? _disposable;

    public WaveRecorder(
        FileInfo fileInfo,
        WaveFormat waveFormat, 
        IObservable<WaveInEventArgs> observable)
    {
        _observable = observable;
        _fileWriter = new WaveFileWriter(fileInfo.FullName, waveFormat);
    }

    public void StartRecording()
    {
        _disposable = _observable.Subscribe(
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