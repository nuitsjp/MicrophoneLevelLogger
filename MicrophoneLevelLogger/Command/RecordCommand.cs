using MicrophoneLevelLogger.Domain;
using MicrophoneLevelLogger.View;

namespace MicrophoneLevelLogger.Command;

public class RecordCommand : ICommand
{
    private readonly IMicrophonesProvider _microphonesProvider;
    private readonly IRecordView _view;

    public RecordCommand(IMicrophonesProvider microphonesProvider, IRecordView view)
    {
        _microphonesProvider = microphonesProvider;
        _view = view;
    }

    public string Name => "Record";


    public Task ExecuteAsync()
    {
        // すべてのマイクを取得する。
        using var microphones = _microphonesProvider.Resolve();

        // 起動時情報を通知する。
        _view.NotifyMicrophonesInformation(microphones);

        // マイクを有効化する
        microphones.Activate();

        // キャプチャーを開始する。
        microphones.StartRecording();

        // 画面に入力レベルを通知する。
        _view.StartNotifyMasterPeakValue(microphones);

        Console.ReadLine();

        // 画面の入力レベル通知を停止する。
        _view.StopNotifyMasterPeakValue();

        // キャプチャーを停止する
        var peakValues = microphones.StopRecording();

        // マイクを無効化する
        microphones.Deactivate();

        // 結果を通知する
        _view.NotifyResult(peakValues);

        return Task.CompletedTask;
    }
}