namespace MicrophoneLevelLogger.Client.Controller.DisplayRecords;

/// <summary>
/// 計測結果表示
/// </summary>
public class DisplayRecordsController : IController
{
    /// <summary>
    /// ビュー
    /// </summary>
    private readonly IDisplayRecordsView _view;
    /// <summary>
    /// IRecordSummaryリポジトリー
    /// </summary>
    private readonly IRecordSummaryRepository _repository;

    /// <summary>
    /// インスタンスを生成する。
    /// </summary>
    /// <param name="view"></param>
    /// <param name="repository"></param>
    public DisplayRecordsController(
        IDisplayRecordsView view, 
        IRecordSummaryRepository repository)
    {
        _view = view;
        _repository = repository;
    }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name => "Display records";
    /// <summary>
    /// 概要
    /// </summary>
    public string Description => "マイク入力レベルの調整結果を表示する。";

    public async Task ExecuteAsync()
    {
        // 計測結果をロードする
        var summaries = await _repository.LoadAsync();

        // 何を表示するか選択する。
        var displayType = _view.SelectDisplayRecordsType();
        if (displayType == DisplayType.RecordView)
        {
            // 計測ごとの表示
            var summary = _view.SelectRecordSummary(summaries);
            _view.Display(summary);
        }
        else
        {
            // マイクごとのサマリー表示
            var microphones = summaries
                // 全記録の下のマイクのサマリー情報を取得する。
                .SelectMany(x => x.Microphones.Select(microphone => (Microphone: microphone, RecordSummary: x)))
                // マイクごとにグルーピングする。
                .GroupBy(x => x.Microphone.Name)
                // マイクごとに最後の記録を取得する。
                .Select(x => x.MaxBy(summary => summary.RecordSummary.Begin).Microphone);
            _view.Display(microphones);
        }
    }
}