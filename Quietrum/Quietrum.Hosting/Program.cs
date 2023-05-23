﻿using Kamishibai;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quietrum;
using Quietrum.Repository;
using Quietrum.View;
using Quietrum.ViewModel;

var builder = KamishibaiApplication<App, MainWindow>.CreateBuilder();

builder.Services.AddTransient<IAudioInterface, AudioInterface>();
builder.Services.AddTransient<ISettingsRepository, SettingsRepository>();

builder.Services.AddPresentation<MainWindow, MainWindowViewModel>();

await builder
    .Build()
    .RunAsync();