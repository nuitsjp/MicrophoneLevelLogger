using MicrophoneNoiseAnalyzer.Command;
using MicrophoneNoiseAnalyzer.Domain;
using MicrophoneNoiseAnalyzer.View;
using Microsoft.Extensions.DependencyInjection;


var builder = ConsoleApp.CreateBuilder(args);
builder.ConfigureServices((ctx, services) =>
{
    services.AddTransient<IMicrophonesProvider, MicrophonesProvider>();
    services.AddTransient<ICalibrateView, CalibrateView>();
    services.AddTransient<IAnalyzeView, AnalyzeView>();
});


var app = builder.Build();
app.AddCommands<CalibrateCommand>();
app.AddCommands<AnalyzeCommand>();
app.Run();