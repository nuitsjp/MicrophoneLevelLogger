using System.Collections.ObjectModel;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kamishibai;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Specter.ViewModel.AnalysisPage;

[Navigate]
public partial class AnalysisPageViewModel : ObservableObject, IAnalyzer, INavigatedAware
{
    private readonly IPresentationService _presentationService;
    private readonly IAudioRecordInterface _audioRecordInterface;
    private readonly ReactiveCollection<AudioRecordViewModel> _audioRecords = new();

    [ObservableProperty] 
    [NotifyCanExecuteChangedFor(nameof(DeleteRecordCommand))]
    private AudioRecordViewModel _selectedAudioRecord;

    public AnalysisPageViewModel(
        [Inject] IPresentationService presentationService,
        [Inject] IAudioRecordInterface audioRecordInterface)
    {
        _presentationService = presentationService;
        _audioRecordInterface = audioRecordInterface;
        AudioRecords = _audioRecords.ToReadOnlyReactiveCollection();
    }

    public ObservableCollection<AnalysisDeviceViewModel> AnalysisDevices { get; } = new(); 
    public ReadOnlyReactiveCollection<AudioRecordViewModel> AudioRecords { get; }

    public void OnNavigated(PostForwardEventArgs args)
    {
        foreach (var audioRecord in _audioRecordInterface.AudioRecords)
        {
            _audioRecords.Add(
                new AudioRecordViewModel(audioRecord, this));
        }

        _audioRecordInterface
            .AudioRecords
            .CollectionChangedAsObservable()
            .ObserveOn(CurrentThreadScheduler.Instance)
            .Subscribe(eventArgs =>
            {
                if (eventArgs.NewItems is not null)
                {
                    foreach (AudioRecord newItem in eventArgs.NewItems)
                    {
                        if (_audioRecords.SingleOrDefault(x => x.AudioRecord == newItem) is null)
                        {
                            _audioRecords.Add(new AudioRecordViewModel(newItem, this));
                        }
                    }
                }

                if (eventArgs.OldItems is not null)
                {
                    foreach (AudioRecord oldItem in eventArgs.OldItems)
                    {
                        _audioRecords.Remove(x => x.AudioRecord == oldItem);
                    }
                }
            });
    }

    [RelayCommand(CanExecute = nameof(CanDeleteRecord))]
    private void DeleteRecord(AudioRecordViewModel? audioRecord)
    {
        var result = _presentationService.ShowMessage(
            "記録を削除します。よろしいですか？",
            button: MessageBoxButton.YesNo);
        if (result == MessageBoxResult.Yes)
        {
            _audioRecordInterface.DeleteAudioRecord(audioRecord.AudioRecord);
        }
    }

    private bool CanDeleteRecord(AudioRecordViewModel? audioRecord)
        => audioRecord is not null;

    public void UpdateTarget(DeviceRecordViewModel deviceRecord)
    {
        if (deviceRecord.Analysis)
        {
            var audioRecord =
                AudioRecords.Single(x => x.Devices.Contains(deviceRecord));
            AnalysisDevices.Add(new AnalysisDeviceViewModel(audioRecord, deviceRecord));
        }
        else
        {
            AnalysisDevices.Remove(x => x.DeviceRecord == deviceRecord);
        }
    }

    public IEnumerable<Decibel> ReadInputLevels(
        AudioRecordViewModel audioRecord,
        DeviceRecordViewModel deviceRecord)
    {
        return _audioRecordInterface.ReadInputLevels(
            audioRecord.AudioRecord, 
            deviceRecord.DeviceRecord);
    }
}