using System.Reactive.Disposables;
using NAudio.Wave;
using Reactive.Bindings.Extensions;
using Specter.Business;

namespace Specter.Repository;

public class DeviceRecorder : IDeviceRecorder
{
    private readonly CompositeDisposable _compositeDisposable = new();
    private readonly IDevice _device;
    private readonly WaveWriter _waveWriter;
    private readonly IDecibelsWriter _inputLevelWriter;
    private readonly List<Decibel> _decibels = new();
    public DeviceRecorder(
        DirectoryInfo parent,
        IDevice device, 
        WaveFormat waveFormat)
    {
        _device = device;
        
        FileInfo _waveFile = new(Path.Combine(parent.FullName, $"{device.Name}.wav"));
        _waveWriter = 
            new WaveWriter(parent.FullName, device, waveFormat)
                .AddTo(_compositeDisposable);

        FileInfo _inputLevelFile = new(Path.Combine(parent.FullName, $"{device.Name}.ilv"));
        _inputLevelWriter =
            new DecibelsWriter(File.Create(_inputLevelFile.FullName))
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
                _inputLevelWriter.Write(x);
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
        _compositeDisposable.Dispose();
        _compositeDisposable.Clear();
    }
}