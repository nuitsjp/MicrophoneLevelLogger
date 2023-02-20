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
    private readonly IRecorderProvider _recorderProvider;
    private readonly IMediaPlayerProvider _mediaPlayerProvider;
    private readonly ISettingsRepository _settingsRepository;

    public RecordController(
        IAudioInterfaceProvider audioInterfaceProvider,
        IRecordView view,
        IMediaPlayerProvider mediaPlayerProvider, 
        ISettingsRepository settingsRepository, 
        IRecorderProvider recorderProvider)
    {
        _view = view;
        _mediaPlayerProvider = mediaPlayerProvider;
        _settingsRepository = settingsRepository;
        _recorderProvider = recorderProvider;
        _audioInterfaceProvider = audioInterfaceProvider;
    }

    public string Name => "Record               : マイクの入力をキャプチャーし保存する。";


    public async Task ExecuteAsync()
    {
        // 録音名を入力する。
        string recordName = _view.InputRecordName();

        var audioInterface = _audioInterfaceProvider.Resolve();
        var recorder = _recorderProvider.ResolveLocal(audioInterface, recordName);

        CancellationTokenSource source = new();

        // 開始を通知する。
        var settings = await _settingsRepository.LoadAsync();
        _view.NotifyStarting(settings.RecordingSpan);

        // 画面に入力レベルを通知する。
        _view.StartNotify(recorder, source.Token);

        try
        {
            // 背景音源を再生する
            var mediaPlayer = _mediaPlayerProvider.Resolve(settings.IsEnableRemotePlaying);
            await mediaPlayer.PlayLoopingAsync(source.Token);


            // 録音を開始する。
            // リモート録音が有効な場合、初回開始に時間がかかる事があるためリモートを先に開始する。
            if (settings.IsEnableRemoteRecording)
            {
                var remoteLogger = _recorderProvider.ResolveRemote(recordName);
                await remoteLogger.StartAsync(source.Token);
            }
            await recorder.StartAsync(source.Token);

            // 録音時間、待機する。
            _view.Wait(settings.RecordingSpan);

            // 録音結果を通知する。
            _view.NotifyResult(recorder);
        }
        finally
        {
            source.Cancel();
        }

    }
}
