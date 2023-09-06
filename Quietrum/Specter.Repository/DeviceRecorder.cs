using System.Reactive.Disposables;
using NAudio.Wave;
using Reactive.Bindings.Extensions;
using Specter.Business;

namespace Specter.Repository;

public class DeviceRecorder : IDeviceRecorder
{
    private readonly CompositeDisposable _compositeDisposable = new();
    private readonly FileInfo _waveFile;
    private readonly FileInfo _inputLevelFile;
    private readonly IDevice _device;
    private readonly WaveFileWriter _waveWriter;
    private readonly BinaryWriter _inputLevelWriter;
    private readonly List<Decibel> _decibels = new();
    public DeviceRecorder(
        DirectoryInfo parent,
        IDevice device, 
        WaveFormat waveFormat)
    {
        _device = device;
        
        var directoryInfo = new DirectoryInfo(Path.Combine(parent.FullName, _device.Name));
        directoryInfo.Create();

        _waveFile = new(Path.Combine(directoryInfo.FullName, "record.wav"));
        _waveWriter = 
            new WaveFileWriter(_waveFile.FullName, waveFormat)
                .AddTo(_compositeDisposable);

        _inputLevelFile = new(Path.Combine(directoryInfo.FullName, "record.ilv"));
        _inputLevelWriter =
            new BinaryWriter(File.Create(_inputLevelFile.FullName))
                .AddTo(_compositeDisposable);
    }

    public void Start()
    {
        _device.WaveInput
            .Subscribe(
                onNext: e => _waveWriter.Write(e.Buffer, 0, e.BytesRecorded),
                onCompleted: OnCompleted)
            .AddTo(_compositeDisposable);
        _device.InputLevel
            .Subscribe(x =>
            {
                _inputLevelWriter.Write(x.AsPrimitive());
                _decibels.Add(x);
            })
            .AddTo(_compositeDisposable);
    }

    public DeviceRecord Stop()
    {
        OnCompleted();
        return new(
            _device.Id,
            _device.Name,
            _device.SystemName,
            _waveFile,
            _inputLevelFile,
            _decibels.Min(),
            new Decibel(_decibels.Average(x => x.AsPrimitive())),
            _decibels.Max(),
            (double)_decibels.Count(x => -30d < x.AsPrimitive()) / _decibels.Count,
            (double)_decibels.Count(x => -40d < x.AsPrimitive()) / _decibels.Count,
            (double)_decibels.Count(x => -50d < x.AsPrimitive()) / _decibels.Count
        );
    }
    
    private void OnCompleted()
    {
        _waveWriter.Flush();
        _inputLevelWriter.Flush();
        _compositeDisposable.Dispose();
        _compositeDisposable.Clear();
    }
}

public interface IDecibelsWriter
{
    
}