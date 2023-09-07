using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Specter.Business;

namespace Specter.ViewModel;

public partial class AnalysisPageViewModel : ObservableObject, IAnalyzer
{
    private readonly IAudioRecordRepository _audioRecordRepository;
    private readonly IDecibelsReaderProvider _decibelsReaderProvider;

    [ObservableProperty] private List<AudioRecordViewModel> _audioRecords = new();
    [ObservableProperty] private AudioRecordViewModel _selectedAudioRecord;

    public AnalysisPageViewModel(
        IAudioRecordRepository audioRecordRepository, 
        IDecibelsReaderProvider decibelsReaderProvider)
    {
        _audioRecordRepository = audioRecordRepository;
        _decibelsReaderProvider = decibelsReaderProvider;
    }

    public ObservableCollection<AnalysisDeviceViewModel> AnalysisDevices { get; } = new(); 

    [RelayCommand]
    private async Task ReloadAsync()
    {
        AudioRecords =
            (await _audioRecordRepository.LoadAsync())
            .Select(x => new AudioRecordViewModel(x, this))
            .ToList();
    }

    public void UpdateTarget(DeviceRecordViewModel deviceRecord)
    {
        if (deviceRecord.Analysis)
        {
            var audioRecord =
                AudioRecords.Single(x => x.Devices.Contains(deviceRecord));
            AnalysisDevices.Add(
                new AnalysisDeviceViewModel(
                    audioRecord, 
                    deviceRecord,
                    _decibelsReaderProvider));
        }
        else
        {
            AnalysisDevices.Remove(x => x.DeviceRecord == deviceRecord);
        }
    }
}

public class AnalysisDeviceViewModel
{
    private readonly IDecibelsReaderProvider _decibelsReaderProvider;
    public AnalysisDeviceViewModel(
        AudioRecordViewModel audioRecord, 
        DeviceRecordViewModel deviceRecord, 
        IDecibelsReaderProvider decibelsReaderProvider)
    {
        AudioRecord = audioRecord;
        DeviceRecord = deviceRecord;
        _decibelsReaderProvider = decibelsReaderProvider;
    }
    public AudioRecordViewModel AudioRecord { get; }
    public DeviceRecordViewModel DeviceRecord { get; }
    public DateTime StartTime => AudioRecord.StartTime;
    public string Device => DeviceRecord.Name;
    public Direction Direction => AudioRecord.Direction;
    public Decibel Min => DeviceRecord.Min;
    public Decibel Avg => DeviceRecord.Avg;
    public Decibel Max => DeviceRecord.Max;
    public double Minus30db => DeviceRecord.Minus30db;
    public double Minus40db => DeviceRecord.Minus40db;
    public double Minus50db => DeviceRecord.Minus50db;

    public bool Analysis
    {
        get => DeviceRecord.Analysis;
        set => DeviceRecord.Analysis = value;
    }

    public IEnumerable<Decibel> GetInputLevels()
    {
        var reader = _decibelsReaderProvider.Provide(
            AudioRecord.AudioRecord,
            DeviceRecord.DeviceRecord);
        return reader.Read();
    }
}

public interface IAnalyzer
{
    void UpdateTarget(DeviceRecordViewModel deviceRecord);
}

public partial class AudioRecordViewModel : ObservableObject
{
    private readonly AudioRecord _audioRecord;

    [ObservableProperty] private DeviceRecordViewModel _selectedDevice;

    public AudioRecordViewModel(AudioRecord audioRecord, IAnalyzer analyzer)
    {
        _audioRecord = audioRecord;
        Device = _audioRecord
            .DeviceRecords
            .Single(x => x.Id == _audioRecord.TargetDeviceId)
            .Name;
        Devices = _audioRecord
            .DeviceRecords
            .Select(x => new DeviceRecordViewModel(x, analyzer))
            .ToList();
    }

    public AudioRecord AudioRecord => _audioRecord;
    public DateTime StartTime => _audioRecord.StartTime;
    public string Device { get; }
    public Direction Direction => _audioRecord.Direction;
    public IReadOnlyList<DeviceRecordViewModel> Devices { get; }
}

public partial class DeviceRecordViewModel
{
    private readonly IAnalyzer _analyzer;
    private readonly DeviceRecord _deviceRecord;

    private bool _analysis;

    public DeviceRecordViewModel(DeviceRecord deviceRecord, IAnalyzer analyzer)
    {
        _deviceRecord = deviceRecord;
        _analyzer = analyzer;
    }

    public DeviceRecord DeviceRecord => _deviceRecord;
    public string Name => _deviceRecord.Name;
    public Decibel Min => _deviceRecord.Min;
    public Decibel Avg => _deviceRecord.Avg;
    public Decibel Max => _deviceRecord.Max;
    public double Minus30db => _deviceRecord.Minus30db;
    public double Minus40db => _deviceRecord.Minus40db;
    public double Minus50db => _deviceRecord.Minus50db;

    public bool Analysis
    {
        get => _analysis;
        set
        {
            _analysis = value;
            _analyzer.UpdateTarget(this);
        }
    }
}