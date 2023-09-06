using System.Reactive.Disposables;
using NAudio.Wave;
using Reactive.Bindings.Extensions;
using Specter.Business;

namespace Specter.Repository;

public class DeviceRecorder : IDeviceRecorder
{
    private readonly CompositeDisposable _compositeDisposable = new();
    private readonly DirectoryInfo _directoryInfo;
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
        
        _directoryInfo = new DirectoryInfo(Path.Combine(parent.FullName, _device.Name));
        _directoryInfo.Create();
        
        _waveWriter = 
            new WaveFileWriter(Path.Combine(_directoryInfo.FullName, "input.wav"), waveFormat)
                .AddTo(_compositeDisposable);
        _inputLevelWriter =
            new BinaryWriter(
                    File.Create(Path.Combine(_directoryInfo.FullName, "input.bin")))
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
        return new(_directoryInfo, _device.Name, _decibels.ToArray());
    }
    
    private void OnCompleted()
    {
        _waveWriter.Flush();
        _inputLevelWriter.Flush();
        _compositeDisposable.Dispose();
        _compositeDisposable.Clear();
    }
}