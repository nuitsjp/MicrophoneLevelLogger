using System.Reactive.Disposables;
using CommunityToolkit.Mvvm.ComponentModel;
using Kamishibai;

namespace Specter.ViewModel;

[Navigate]
public partial class SettingsPageViewModel : ObservableObject, INavigatedAsyncAware, IDisposable
{
    private readonly CompositeDisposable _compositeDisposable = new();
    private readonly ISettingsRepository _settingsRepository;
    private readonly IFastFourierTransformSettings _fastFourierTransformSettings;

    public bool EnableAWeighting
    {
        get => _fastFourierTransformSettings.EnableAWeighting.Value;
        set
        {
            _fastFourierTransformSettings.EnableAWeighting.Value = value;
            UpdateSettings();
            OnPropertyChanged();
        }
    }

    public bool EnableFastTimeWeighting
    {
        get => _fastFourierTransformSettings.EnableFastTimeWeighting.Value;
        set
        {
            _fastFourierTransformSettings.EnableFastTimeWeighting.Value = value;
            UpdateSettings();
            OnPropertyChanged();
        }
    }
    
    public SettingsPageViewModel(
        [Inject] ISettingsRepository settingsRepository, 
        [Inject] IFastFourierTransformSettings fastFourierTransformSettings)
    {
        _settingsRepository = settingsRepository;
        _fastFourierTransformSettings = fastFourierTransformSettings;
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
    }

    public void Dispose()
    {
        _compositeDisposable.Dispose();
    }
}