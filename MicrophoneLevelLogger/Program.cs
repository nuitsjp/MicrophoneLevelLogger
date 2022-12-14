using MicrophoneLevelLogger.Command;
using MicrophoneLevelLogger.Domain;
using MicrophoneLevelLogger.View;
using Microsoft.Extensions.DependencyInjection;


var builder = ConsoleApp.CreateBuilder(args);
builder.ConfigureServices((ctx, services) =>
{
    services.AddTransient<IMicrophonesProvider, MicrophonesProvider>();
    services.AddTransient<ICalibrateView, CalibrateView>();
    services.AddTransient<IRecordView, RecordView>();
});


var app = builder.Build();
app.AddCommands<CalibrateCommand>();
app.AddCommands<RecordCommand>();
app.Run();