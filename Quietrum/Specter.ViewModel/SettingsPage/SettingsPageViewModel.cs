using System.Reactive.Disposables;
using CommunityToolkit.Mvvm.ComponentModel;
using Kamishibai;
using Reactive.Bindings.Extensions;

namespace Specter.ViewModel;

[Navigate]
public partial class SettingsPageViewModel : ObservableObject, INavigatedAsyncAware, IDisposable
{
    private readonly CompositeDisposable _compositeDisposable = new();
    private readonly ISettingsRepository _settingsRepository;

    public bool EnableAWeighting
    {
        get => FastFourierTransformSettings.EnableAWeighting.Value;
        set
        {
            FastFourierTransformSettings.EnableAWeighting.Value = value;
            UpdateSettings();
            OnPropertyChanged();
        }
    }

    public bool EnableFastTimeWeighting
    {
        get => FastFourierTransformSettings.EnableFastTimeWeighting.Value;
        set
        {
            FastFourierTransformSettings.EnableFastTimeWeighting.Value = value;
            UpdateSettings();
            OnPropertyChanged();
        }
    }
    
    public SettingsPageViewModel(
        [Inject] ISettingsRepository settingsRepository)
    {
        _settingsRepository = settingsRepository;
    }

    public async Task OnNavigatedAsync(PostForwardEventArgs args)
    {
        var settings = await _settingsRepository.LoadAsync();
        
        EnableAWeighting = settings.EnableAWeighting;
        EnableFastTimeWeighting = settings.EnableFastTimeWeighting;
    }

    private async void UpdateSettings()
    {
        var settings = await _settingsRepository.LoadAsync();
        await _settingsRepository.SaveAsync(settings with
        {
            EnableAWeighting = EnableAWeighting,
            EnableFastTimeWeighting = EnableFastTimeWeighting
        });
        FastFourierTransformSettings.EnableFastTimeWeighting.Value = EnableFastTimeWeighting;
    }

    public void Dispose()
    {
        _compositeDisposable.Dispose();
    }
}