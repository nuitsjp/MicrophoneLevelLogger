namespace MicrophoneLevelLogger.Client.Controller.Record;

public class RecordController : IController
{
    public const string RecordDirectoryName = "Record";

    private readonly IRecordView _view;
    private readonly IAudioInterfaceProvider _audioInterfaceProvider;
    private readonly IRecorderProvider _recorderProviderProvider;
    private readonly IMediaPlayerProvider _mediaPlayerProvider;

    public RecordController(
        IAudioInterfaceProvider audioInterfaceProvider,
        IRecordView view,
        IRecorderProvider recorderProvider,
        IMediaPlayerProvider mediaPlayerProvider)
    {
        _view = view;
        _recorderProviderProvider = recorderProvider;
        _mediaPlayerProvider = mediaPlayerProvider;
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

        var settings = await MicrophoneLevelLogger.RecordingSettings.LoadAsync();
        _view.NotifyStarting(settings.RecordingSpan);

        // 画面に入力レベルを通知する。
        _view.StartNotifyMasterPeakValue(audioInterface);

        // 背景音源を再生する
        var mediaPlayer = settings.IsEnableRemotePlaying
            ? _mediaPlayerProvider.ResolveRemote()
            : _mediaPlayerProvider.ResolveLocale();
        await mediaPlayer.PlayLoopingAsync();

        var localRecorder = _recorderProviderProvider.ResolveLocal();
        var remoteRecorder = _recorderProviderProvider.ResolveRemote();
        try
        {
            if (settings.IsEnableRemoteRecording)
            {
                await remoteRecorder.RecodeAsync(recordName);
            }
            await localRecorder.RecodeAsync(recordName);

            // 録音時間、待機する。
            await Task.Delay(settings.RecordingSpan);

        }
        finally
        {
            // 画面の入力レベル通知を停止する。
            _view.StopNotifyMasterPeakValue();

            if (settings.IsEnableRemoteRecording)
            {
                await remoteRecorder.StopQuietAsync();
            }
            await localRecorder.StopQuietAsync();
            await mediaPlayer.StopAsync();
        }

    }
}

public static class RecorderExtensions
{
    public static async Task StopQuietAsync(this IRecorder recorder)
    {
        try
        {
            await recorder.StopAsync();
        }
        catch
        {
            // ignore
        }
    }
}