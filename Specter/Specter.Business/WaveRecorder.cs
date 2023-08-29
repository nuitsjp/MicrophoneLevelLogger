using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using NAudio.Wave;
using Reactive.Bindings.Extensions;

namespace Specter.Business;

public class WaveRecorder
{
    private readonly IDevice _device;
    private readonly WaveFileWriter _waveWriter;
    private readonly BinaryWriter _inputLevelWriter;
    private readonly CompositeDisposable _compositeDisposable = new();
    private readonly ObservableCollection<Decibel> _decibels = new();

    private readonly DirectoryInfo _directoryInfo;

    private readonly IWaveRecordIndexRepository _indexRepository;

    public WaveRecorder(
        IDevice device,
        WaveFormat waveFormat, 
        DirectoryInfo directoryInfo, 
        IWaveRecordIndexRepository indexRepository)
    {
        if (directoryInfo.Exists is false)
        {
            directoryInfo.Create();
        }

        _device = device;
        _directoryInfo = directoryInfo;
        _indexRepository = indexRepository;
        _waveWriter = 
            new WaveFileWriter(Path.Combine(directoryInfo.FullName, $"{_device.Name}.wav"), waveFormat)
                .AddTo(_compositeDisposable);
        _inputLevelWriter =
            new BinaryWriter(
                File.Create(Path.Combine(directoryInfo.FullName, $"{_device.Name}.bin")))
                .AddTo(_compositeDisposable);
        Decibels = new ReadOnlyObservableCollection<Decibel>(_decibels);
    }

    public ReadOnlyObservableCollection<Decibel> Decibels { get; }

    public void StartRecording()
    {
        _device.WaveInput
            .Subscribe(
                onNext: e => _waveWriter.Write(e.Buffer, 0, e.BytesRecorded),
                onCompleted: OnCompleted)
            .AddTo(_compositeDisposable);
        _decibels.Clear();
        _device.InputLevel
            .Subscribe(x =>
            {
                _inputLevelWriter.Write(x.AsPrimitive());
                _decibels.Add(x);
            })
            .AddTo(_compositeDisposable);
    }

    public void StopRecording()
    {
        OnCompleted();
    }

    private async void OnCompleted()
    {
        _waveWriter.Flush();
        _inputLevelWriter.Flush();
        await _indexRepository.SaveAsync(
            new(Path.Combine(_directoryInfo.FullName, "index.json")),
            new(
                _device.Id,
                _device.DataFlow,
                _device.Name,
                _device.SystemName));
        _compositeDisposable.Dispose();
        _compositeDisposable.Clear();
    }
}