using CommunityToolkit.Mvvm.ComponentModel;

namespace Specter.ViewModel.AnalysisPage;

public partial class AudioRecordViewModel : ObservableObject
{
    private readonly AudioRecord _audioRecord;

    [ObservableProperty] private DeviceRecordViewModel? _selectedDevice;

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
    public RecordingMethod RecordingMethod => _audioRecord.RecordingMethod;
    public IReadOnlyList<DeviceRecordViewModel> Devices { get; }
}