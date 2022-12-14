using MicrophoneLevelLogger.Domain;
using MicrophoneLevelLogger.View;

namespace MicrophoneLevelLogger.Command;

public class AnalyzeCommand : ConsoleAppBase
{
    private readonly IMicrophonesProvider _microphonesProvider;
    private readonly IAnalyzeView _view;

    public AnalyzeCommand(IMicrophonesProvider microphonesProvider, IAnalyzeView view)
    {
        _microphonesProvider = microphonesProvider;
        _view = view;
    }

    public void Analyze()
    {
        // すべてのマイクを取得する。
        using IMicrophones microphones = _microphonesProvider.Resolve();

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
        var peakValuesList = microphones.StopRecording().ToList();

        // マイクを無効化する
        microphones.Deactivate();
    }

}