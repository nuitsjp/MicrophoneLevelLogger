using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Specter.Business;

namespace Specter.ViewModel;

public partial class AnalysisPageViewModel : ObservableObject
{
    private readonly IAudioRecordRepository _audioRecordRepository;

    [ObservableProperty] private List<AudioRecordViewModel> _audioRecords = new();
    [ObservableProperty] private AudioRecordViewModel? _selectedAudioRecord;

    public AnalysisPageViewModel(IAudioRecordRepository audioRecordRepository)
    {
        _audioRecordRepository = audioRecordRepository;
    }

    [RelayCommand]
    private async Task ReloadAsync()
    {
        AudioRecords =
            (await _audioRecordRepository.LoadAsync())
            .Select(x => new AudioRecordViewModel(x))
            .ToList();
    }
}

public class AudioRecordViewModel : ObservableObject
{
    private readonly AudioRecord _audioRecord;

    public AudioRecordViewModel(AudioRecord audioRecord)
    {
        _audioRecord = audioRecord;
        Device = _audioRecord
            .DeviceRecords
            .Single(x => x.Id == _audioRecord.TargetDeviceId)
            .Name;
        Devices = _audioRecord
            .DeviceRecords
            .Select(x => new DeviceRecordViewModel(x))
            .ToList();
    }

    public DateTime StartTime => _audioRecord.StartTime;
    public string Device { get; }
    public Direction Direction => _audioRecord.Direction;
    public IReadOnlyList<DeviceRecordViewModel> Devices { get; }
}

public class DeviceRecordViewModel
{
    private readonly DeviceRecord _deviceRecord;

    public DeviceRecordViewModel(DeviceRecord deviceRecord)
    {
        _deviceRecord = deviceRecord;
    }

    public string Name => _deviceRecord.Name;
    public Decibel Min => _deviceRecord.Min;
    public Decibel Avg => _deviceRecord.Avg;
    public Decibel Max => _deviceRecord.Max;
    public double Minus30db => _deviceRecord.Minus30db;
    public double Minus40db => _deviceRecord.Minus40db;
    public double Minus50db => _deviceRecord.Minus50db;
}