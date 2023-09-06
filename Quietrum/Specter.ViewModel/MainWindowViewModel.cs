using CommunityToolkit.Mvvm.ComponentModel;
using Kamishibai;
using Reactive.Bindings.Disposables;
using Specter.Business;

namespace Specter.ViewModel;

[Navigate]
public partial class MainWindowViewModel : ObservableObject, INavigatedAsyncAware, IDisposable
{
    private readonly CompositeDisposable _compositeDisposable = new();

    public MainWindowViewModel(
        [Inject] IAudioInterfaceProvider audioInterfaceProvider,
        [Inject] IAudioRecorderProvider audioRecorderProvider, 
        [Inject] ISettingsRepository settingsRepository)
    {
        MonitoringPage =
            new MonitoringPageViewModel(
                audioInterfaceProvider,
                audioRecorderProvider,
                settingsRepository);
    }
    
    public MonitoringPageViewModel MonitoringPage { get; }


    public void Dispose()
    {
        _compositeDisposable.Dispose();
    }

    public async Task OnNavigatedAsync(PostForwardEventArgs args)
    {
        await MonitoringPage.OnNavigatedAsync(args);
    }
}