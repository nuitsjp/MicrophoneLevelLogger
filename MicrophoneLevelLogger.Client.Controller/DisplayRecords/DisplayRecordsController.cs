namespace MicrophoneLevelLogger.Client.Controller.DisplayRecords;

public class DisplayRecordsController : IController
{
    private readonly IDisplayRecordsView _view;
    private readonly IRecordSummaryRepository _repository;

    public DisplayRecordsController(
        IDisplayRecordsView view, 
        IRecordSummaryRepository repository)
    {
        _view = view;
        _repository = repository;
    }

    public string Name => "Display records      : 計測結果を表示する。";
    public async Task ExecuteAsync()
    {
        var summaries = await _repository.LoadAsync();
        var displayType = _view.SelectDisplayRecordsType();
        if (displayType == DisplayType.RecordView)
        {
            var summary = _view.SelectRecordSummary(summaries);
            _view.Display(summary);
        }
        else
        {
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