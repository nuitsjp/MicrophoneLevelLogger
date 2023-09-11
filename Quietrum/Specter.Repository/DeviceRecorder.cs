using System.Reactive.Disposables;
using NAudio.Wave;
using Reactive.Bindings.Extensions;

namespace Specter.Repository;

public class DeviceRecorder
{
    private readonly CompositeDisposable _compositeDisposable = new();
    private readonly IDevice _device;
    private readonly IWaveWriter _waveWriter;
    private readonly BinaryWriter _inputLevelWriter;
    private readonly List<Decibel> _decibels = new();
    public DeviceRecorder(IDevice device, IWaveWriter waveWriter, BinaryWriter inputLevelWriter)
    {
        _device = device;
        _waveWriter = waveWriter;
        _inputLevelWriter = inputLevelWriter;
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
        if (_decibels.Any())
        {
            return new(
                _device.Id,
                _device.Name,
                _device.SystemName,
                _decibels.Min(),
                new Decibel(_decibels.Average(x => x.AsPrimitive())),
                _decibels.Max(),
                (double)_decibels.Count(x => -30d < x.AsPrimitive()) / _decibels.Count,
                (double)_decibels.Count(x => -40d < x.AsPrimitive()) / _decibels.Count,
                (double)_decibels.Count(x => -50d < x.AsPrimitive()) / _decibels.Count);
        }
        else
        {
            return new(
                _device.Id,
                _device.Name,
                _device.SystemName,
                Decibel.Minimum,
                Decibel.Minimum,
                Decibel.Minimum,
                0,
                0,
                0);
        }
    }
    
    private void OnCompleted()
    {
        _compositeDisposable.Dispose();
        _compositeDisposable.Clear();
    }
}