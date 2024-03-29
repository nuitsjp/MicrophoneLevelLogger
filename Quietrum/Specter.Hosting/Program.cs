﻿using Kamishibai;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Specter;
using Specter.Repository;
using Specter.View;
using Specter.ViewModel;
using Specter.ViewModel.AnalysisPage;
using Specter.ViewModel.MonitoringPage;

var builder = KamishibaiApplication<App, MainWindow>.CreateBuilder();

builder.Services.AddSingleton<IAudioInterfaceProvider, AudioInterfaceProvider>();
builder.Services.AddSingleton<IAudioRecordInterface, AudioRecordInterface>();
builder.Services.AddTransient<ISettingsRepository, SettingsRepository>();
builder.Services.AddTransient<IFastFourierTransformSettings, FastFourierTransformSettings>();

builder.Services.AddPresentation<MainWindow, MainWindowViewModel>();
builder.Services.AddPresentation<MonitoringPage, MonitoringPageViewModel>();
builder.Services.AddPresentation<AnalysisPage, AnalysisPageViewModel>();
builder.Services.AddPresentation<SettingsPage, SettingsPageViewModel>();

await builder
    .Build()
    .RunAsync();