using MicrophoneLevelLogger.Client.Controller.Record;
using MicrophoneLevelLogger.Client.View;
using MicrophoneLevelLogger;
using MicrophoneLevelLogger.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddTransient<IAudioInterfaceProvider, AudioInterfaceProvider>();
builder.Services.AddTransient<IAudioInterfaceLoggerProvider, AudioInterfaceLoggerProvider>();
builder.Services.AddTransient<IRecordingSettingsRepository, RecordingSettingsRepository>();
builder.Services.AddSingleton<IMediaPlayer, MediaPlayer>();
builder.Services.AddSingleton<IRecorder, Recorder>();
builder.Services.AddTransient<IRecordView, RecordView>();

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Run();
