using MicrophoneNoiseAnalyzer.Command;
using MicrophoneNoiseAnalyzer.Domain;
using MicrophoneNoiseAnalyzer.View;
using Microsoft.Extensions.DependencyInjection;


var builder = ConsoleApp.CreateBuilder(args);
builder.ConfigureServices((ctx, services) =>
{
    services.AddTransient<IMicrophonesProvider, MicrophonesProvider>();
    services.AddTransient<ICalibrationView, CalibrationView>();
});


var app = builder.Build();
app.AddCommands<MicrophoneNoiseAnalyzerCommands>();
app.Run();