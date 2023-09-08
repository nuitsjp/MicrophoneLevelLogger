using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Specter.ViewModel.AnalysisPage;

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