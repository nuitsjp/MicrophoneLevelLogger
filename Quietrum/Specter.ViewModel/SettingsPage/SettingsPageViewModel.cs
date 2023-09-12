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
    [ObservableProperty] private bool _enableAWeighting = true;
    [ObservableProperty] private bool _enableFastTimeWeighting = true;

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
        
        FastFourierTransformSettings.EnableAWeighting.Value = EnableAWeighting;
        FastFourierTransformSettings.EnableFastTimeWeighting.Value = EnableFastTimeWeighting;

        this.ObserveProperty(x => x.EnableAWeighting)
            .Subscribe(OnNextEnableAWeighting)
            .AddTo(_compositeDisposable);

        this.ObserveProperty(x => x.EnableFastTimeWeighting)
            .Subscribe(OnNextEnableFastTimeWeighting)
            .AddTo(_compositeDisposable);
    }

    private async void OnNextEnableAWeighting(bool value)
    {
        var settings = await _settingsRepository.LoadAsync();
        await _settingsRepository.SaveAsync(settings with
        {
            EnableAWeighting = EnableAWeighting
        });
        FastFourierTransformSettings.EnableAWeighting.Value = EnableAWeighting;
    }

    private async void OnNextEnableFastTimeWeighting(bool value)
    {
        var settings = await _settingsRepository.LoadAsync();
        await _settingsRepository.SaveAsync(settings with
        {
            EnableFastTimeWeighting = EnableFastTimeWeighting
        });
        FastFourierTransformSettings.EnableFastTimeWeighting.Value = EnableFastTimeWeighting;
    }

    public void Dispose()
    {
        _compositeDisposable.Dispose();
    }
}