namespace MicrophoneLevelLogger.Client.Controller.Record;

/// <summary>
/// 記録する。
/// </summary>
public class RecordController : IController
{
    /// <summary>
    /// データを保管するルートディレクトリー名
    /// </summary>
    public const string RecordDirectoryName = "Record";

    /// <summary>
    /// ビュー
    /// </summary>
    private readonly IRecordView _view;
    /// <summary>
    /// IAudioInterfaceプロバイダー
    /// </summary>
    private readonly IAudioInterfaceProvider _audioInterfaceProvider;
    /// <summary>
    /// IRecorderプロバイダー
    /// </summary>
    private readonly IRecorderProvider _recorderProvider;
    /// <summary>
    /// Settingsリポジトリー
    /// </summary>
    private readonly ISettingsRepository _settingsRepository;

    /// <summary>
    /// インスタンスを生成する。
    /// </summary>
    /// <param name="audioInterfaceProvider"></param>
    /// <param name="view"></param>
    /// <param name="settingsRepository"></param>
    /// <param name="recorderProvider"></param>
    public RecordController(
        IAudioInterfaceProvider audioInterfaceProvider,
        IRecordView view,
        ISettingsRepository settingsRepository, 
        IRecorderProvider recorderProvider)
    {
        _view = view;
        _settingsRepository = settingsRepository;
        _recorderProvider = recorderProvider;
        _audioInterfaceProvider = audioInterfaceProvider;
    }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name => "Record";
    /// <summary>
    /// 概要
    /// </summary>
    public string Description => "マイクの入力をキャプチャーし保存する。";


    public async Task ExecuteAsync()
    {
        // 記録名を入力する。
        string recordName = _view.InputRecordName();

        // レコーダーを解決する。
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
            IMediaPlayer mediaPlayer = settings.IsEnableRemotePlaying
                ? new RemoteMediaPlayer(settings.MediaPlayerHost)
                : new MediaPlayer(await audioInterface.GetSpeakerAsync());
            await mediaPlayer.PlayLoopingAsync(source.Token);


            // 記録を開始する。
            // リモート記録が有効な場合、初回開始に時間がかかる事があるためリモートを先に開始する。
            if (settings.IsEnableRemoteRecording)
            {
                var remoteLogger = _recorderProvider.ResolveRemote(recordName);
                await remoteLogger.StartAsync(source.Token);
            }
            await recorder.StartAsync(source.Token);

            // 記録時間、待機する。
            _view.Wait(settings.RecordingSpan);

            // 記録結果を通知する。
            _view.NotifyResult(recorder);
        }
        finally
        {
            // 記録を中断する。
            source.Cancel();
        }

    }
}
