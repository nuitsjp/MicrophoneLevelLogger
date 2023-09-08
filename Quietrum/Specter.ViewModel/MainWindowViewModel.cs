using CommunityToolkit.Mvvm.ComponentModel;
using Kamishibai;
using Reactive.Bindings.Disposables;

namespace Specter.ViewModel;

[Navigate]
public partial class MainWindowViewModel : ObservableObject, INavigatedAsyncAware, IDisposable
{
    private readonly CompositeDisposable _compositeDisposable = new();

    public MainWindowViewModel(
        [Inject] IAudioInterfaceProvider audioInterfaceProvider,
        [Inject] IAudioRecorderProvider audioRecorderProvider, 
        [Inject] ISettingsRepository settingsRepository,
        [Inject] IAudioRecordRepository audioRecordRepository,
        [Inject] IDecibelsReaderProvider decibelsReaderProvider)
    {
        MonitoringPage =
            new (
                audioInterfaceProvider,
                audioRecorderProvider,
                settingsRepository);
        AnalysisPage = 
            new(
                audioRecordRepository,
                decibelsReaderProvider);
    }
    
    public MonitoringPageViewModel MonitoringPage { get; }
    public AnalysisPageViewModel AnalysisPage { get; }


    public void Dispose()
    {
        _compositeDisposable.Dispose();
    }

    public async Task OnNavigatedAsync(PostForwardEventArgs args)
    {
        await MonitoringPage.OnNavigatedAsync(args);
    }
}