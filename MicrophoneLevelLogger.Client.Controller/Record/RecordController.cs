using CsvHelper;
using FftSharp.Windows;
using NAudio.Wave;
using System.Xml.Linq;

namespace MicrophoneLevelLogger.Client.Controller.Record;

public class RecordController : IController
{
    public const string RecordDirectoryName = "Record";

    private readonly IRecordView _view;
    private readonly IAudioInterfaceProvider _audioInterfaceProvider;
    private readonly IAudioInterfaceLoggerProvider _audioInterfaceLoggerProvider;
    private readonly IMediaPlayerProvider _mediaPlayerProvider;
    private readonly IRecordingSettingsRepository _recordingSettingsRepository;

    public RecordController(
        IAudioInterfaceProvider audioInterfaceProvider,
        IRecordView view,
        IMediaPlayerProvider mediaPlayerProvider, 
        IRecordingSettingsRepository recordingSettingsRepository, 
        IAudioInterfaceLoggerProvider audioInterfaceLoggerProvider)
    {
        _view = view;
        _mediaPlayerProvider = mediaPlayerProvider;
        _recordingSettingsRepository = recordingSettingsRepository;
        _audioInterfaceLoggerProvider = audioInterfaceLoggerProvider;
        _audioInterfaceProvider = audioInterfaceProvider;
    }

    public string Name => "Record               : マイクの入力をキャプチャーし保存する。";


    public async Task ExecuteAsync()
    {
        var audioInterface = _audioInterfaceProvider.Resolve();

        // 起動時情報を通知する。
        _view.NotifyMicrophonesInformation(audioInterface);

        // 録音名を入力する。
        string recordName = _view.InputRecordName();

        var settings = await _recordingSettingsRepository.LoadAsync();
        _view.NotifyStarting(settings.RecordingSpan);

        var logger = _audioInterfaceLoggerProvider.ResolveLocal(audioInterface, recordName);


        CancellationTokenSource source = new();
        try
        {
            // 背景音源を再生する
            var mediaPlayer = _mediaPlayerProvider.Resolve(settings.IsEnableRemotePlaying);
            await mediaPlayer.PlayLoopingAsync(source.Token);

            await logger.StartAsync(source.Token);
            // 画面に入力レベルを通知する。
            _view.StartNotify(logger, source.Token);

            if (settings.IsEnableRemoteRecording)
            {
                var remoteLogger = _audioInterfaceLoggerProvider.ResolveRemote(recordName);
                await remoteLogger.StartAsync(source.Token);
            }

            // 録音時間、待機する。
            _view.Wait(settings.RecordingSpan);

            var results = logger.MicrophoneLoggers
                .Select((x, index) => new RecordResult(index + 1, x))
                .ToList();
            _view.NotifyResult(results);
        }
        finally
        {
            source.Cancel();
        }

    }
}
