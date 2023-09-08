using CommunityToolkit.Mvvm.ComponentModel;
using Kamishibai;
using Reactive.Bindings.Disposables;
using Specter.ViewModel.AnalysisPage;
using Specter.ViewModel.MonitoringPage;

namespace Specter.ViewModel;

[Navigate]
public partial class MainWindowViewModel : ObservableObject, INavigatedAsyncAware, IDisposable
{
    private readonly CompositeDisposable _compositeDisposable = new();

    public MainWindowViewModel(
        [Inject] IAudioInterfaceProvider audioInterfaceProvider,
        [Inject] IAudioRecorderProvider audioRecorderProvider, 
        [Inject] ISettingsRepository settingsRepository,
        [Inject] IDecibelsReaderProvider decibelsReaderProvider,
        [Inject] IAudioRecordInterface audioRecordInterface)
    {
        MonitoringPage =
            new (
                audioInterfaceProvider,
                audioRecorderProvider,
                settingsRepository);
        AnalysisPage = 
            new(
                decibelsReaderProvider,
                audioRecordInterface);
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