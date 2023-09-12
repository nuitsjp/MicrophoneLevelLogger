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

    private readonly IPresentationService _presentationService;
    private readonly IAudioRecordInterface _audioRecordInterface;
    

    public MainWindowViewModel(
        [Inject] IPresentationService presentationService, 
        [Inject] IAudioRecordInterface audioRecordInterface)
    {
        _presentationService = presentationService;
        _audioRecordInterface = audioRecordInterface;
    }

    public string MonitoringFrameName => "MonitoringFrame";
    public string AnalysisFrameName => "AnalysisFrame";
    public string SettingsFrameName => "SettingsFrame";

    public void Dispose()
    {
        _compositeDisposable.Dispose();
    }

    public async Task OnNavigatedAsync(PostForwardEventArgs args)
    {
        await _audioRecordInterface.ActivateAsync();
        
        await _presentationService.NavigateToMonitoringPageAsync(frameName:MonitoringFrameName);
        await _presentationService.NavigateToAnalysisPageAsync(frameName:AnalysisFrameName);
        await _presentationService.NavigateToSettingsPageAsync(frameName:SettingsFrameName);
    }
}