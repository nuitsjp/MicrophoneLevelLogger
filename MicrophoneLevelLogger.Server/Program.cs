using MicrophoneLevelLogger.Client.Controller.Record;
using MicrophoneLevelLogger.Client.View;
using MicrophoneLevelLogger;
using MicrophoneLevelLogger.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddTransient<IAudioInterfaceProvider, AudioInterfaceProvider>();
builder.Services.AddTransient<IRecorderProvider, RecorderProvider>();
builder.Services.AddTransient<ISettingsRepository, SettingsRepository>();
builder.Services.AddSingleton<IMediaPlayer, MediaPlayer>();
builder.Services.AddTransient<IRecordView, RecordView>();

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Run();
