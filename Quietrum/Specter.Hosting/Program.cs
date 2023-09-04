using Kamishibai;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quietrum.View;
using Specter.Repository;
using Specter.View;
using Specter.ViewModel;
using Specter.Business;

var builder = KamishibaiApplication<App, MainWindow>.CreateBuilder();

builder.Services.AddSingleton<IAudioInterfaceProvider, AudioInterfaceProvider>();
builder.Services.AddTransient<ISettingsRepository, SettingsRepository>();
builder.Services.AddTransient<IWaveRecordIndexRepository, WaveRecordIndexRepository>();

builder.Services.AddPresentation<MainWindow, MainWindowViewModel>();
builder.Services.AddPresentation<MonitoringPage, MonitoringPageViewModel>();
builder.Services.AddPresentation<SettingsPage, SettingsPageViewModel>();

await builder
    .Build()
    .RunAsync();