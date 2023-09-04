using Kamishibai;

namespace Specter.ViewModel;

public class MainWindowViewModel : INavigatedAsyncAware
{
    private readonly IPresentationService _presentationService;

    public MainWindowViewModel(
        [Inject] IPresentationService presentationService)
    {
        _presentationService = presentationService;
    }

    public Task OnNavigatedAsync(PostForwardEventArgs args)
        => _presentationService.NavigateToMonitoringPageAsync();
}