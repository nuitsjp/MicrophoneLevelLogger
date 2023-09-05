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
        [Inject] ISettingsRepository settingsRepository,
        [Inject] IWaveRecordIndexRepository waveRecordIndexRepository)
    {
        MonitoringPage =
            new MonitoringPageViewModel(
                audioInterfaceProvider,
                settingsRepository,
                waveRecordIndexRepository);
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