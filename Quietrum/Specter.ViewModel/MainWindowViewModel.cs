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
        [Inject] ISettingsRepository settingsRepository,
        [Inject] IAudioRecordInterface audioRecordInterface)
    {
        MonitoringPage =
            new (
                audioInterfaceProvider,
                audioRecordInterface,
                settingsRepository);
        AnalysisPage = new(audioRecordInterface);
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