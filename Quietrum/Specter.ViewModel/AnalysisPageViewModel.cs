using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Specter.Business;

namespace Specter.ViewModel;

public partial class AnalysisPageViewModel : ObservableObject
{
    private readonly IAudioRecordRepository _audioRecordRepository;

    [ObservableProperty] private List<AudioRecordViewModel> _audioRecords = new();

    public AnalysisPageViewModel(IAudioRecordRepository audioRecordRepository)
    {
        _audioRecordRepository = audioRecordRepository;
    }

    [RelayCommand]
    private async Task ReloadAsync()
    {
        _audioRecords =
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
    }
}