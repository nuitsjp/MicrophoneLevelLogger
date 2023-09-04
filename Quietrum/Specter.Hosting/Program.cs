using Kamishibai;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Specter.Repository;
using Specter.View;
using Specter.ViewModel;
using Specter.Business;

var builder = KamishibaiApplication<App, MainWindow>.CreateBuilder();

builder.Services.AddTransient<IAudioInterfaceProvider, AudioInterfaceProvider>();
builder.Services.AddTransient<ISettingsRepository, SettingsRepository>();
builder.Services.AddTransient<IWaveRecordIndexRepository, WaveRecordIndexRepository>();

builder.Services.AddPresentation<MainWindow, MainWindowViewModel>();
builder.Services.AddPresentation<MonitoringPage, MonitoringPageViewModel>();

await builder
    .Build()
    .RunAsync();