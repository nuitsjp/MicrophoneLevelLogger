using System.Diagnostics;
using System.Globalization;
using CsvHelper;
using FftSharp.Windows;
using FluentTextTable;
using MicrophoneLevelLogger.Domain;

namespace MicrophoneLevelLogger.Command.Record;

public class RecordCommand : ICommand
{
    public const string RecordDirectoryName = "Record";

    private readonly IRecordView _view;
    private readonly IAudioInterface _audioInterface;
    private readonly IRecorderProvider _recorderProviderProvider;
    private readonly IMediaPlayerProvider _mediaPlayerProvider;

    public RecordCommand(
        IAudioInterfaceProvider audioInterfaceProvider,
        IRecordView view,
        IRecorderProvider recorderProvider,
        IMediaPlayerProvider mediaPlayerProvider)
    {
        _view = view;
        _recorderProviderProvider = recorderProvider;
        _mediaPlayerProvider = mediaPlayerProvider;
        _audioInterface = audioInterfaceProvider.Resolve();
    }

    public string Name => "Record               : マイクの入力をキャプチャーし保存する。";


    public async Task ExecuteAsync()
    {
        // 起動時情報を通知する。
        _view.NotifyMicrophonesInformation(_audioInterface);

        // 録音名を入力する。
        string recordName = _view.InputRecordName();

        var settings = await Domain.RecordingSettings.LoadAsync();
        _view.NotifyStarting(settings.RecordingSpan);

        // 背景音源を再生する
        var mediaPlayer = settings.IsEnableRemotePlaying
            ? _mediaPlayerProvider.ResolveRemoteService()
            : _mediaPlayerProvider.ResolveLocaleService();
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