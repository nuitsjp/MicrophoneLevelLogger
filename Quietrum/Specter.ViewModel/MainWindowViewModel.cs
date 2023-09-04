using System.Reactive.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Kamishibai;
using Reactive.Bindings.Disposables;
using Reactive.Bindings.Extensions;

namespace Specter.ViewModel;

public partial class MainWindowViewModel : ObservableObject, IDisposable
{
    private readonly CompositeDisposable _compositeDisposable = new();

    private readonly IPresentationService _presentationService;

    [ObservableProperty] private MenuItem _selectedMenuItem; 

    public MainWindowViewModel(
        [Inject] IPresentationService presentationService)
    {
        _presentationService = presentationService;
        this.ObserveProperty(x => x.SelectedMenuItem)
            .Where(x => x is not null)
            .Subscribe(x =>
            {
                _presentationService.NavigateAsync(x.ViewModel);
            })
            .AddTo(_compositeDisposable);
        SelectedMenuItem = MenuItems.First();
    }

    public IReadOnlyList<MenuItem> MenuItems { get; } = new List<MenuItem>
    {
        new ("Monitoring", typeof(MonitoringPageViewModel)),
        new ("Settings", typeof(SettingsPageViewModel))
    };

    public void Dispose()
    {
        _compositeDisposable.Dispose();
    }
}