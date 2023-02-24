using MicrophoneLevelLogger.Client.Controller.Record;

namespace MicrophoneLevelLogger.Client.Controller.DeleteRecord;

/// <summary>
/// 録音結果を削除する。
/// </summary>
public class DeleteRecordController : IController
{
    /// <summary>
    /// ビュー
    /// </summary>
    private readonly IDeleteRecordView _view;

    /// <summary>
    /// インスタンスを生成する。
    /// </summary>
    /// <param name="view"></param>
    public DeleteRecordController(IDeleteRecordView view)
    {
        _view = view;
    }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name => "Delete records";
    /// <summary>
    /// 概要
    /// </summary>
    public string Description => "Recodeで保存したデータをすべて削除する。";

    public Task ExecuteAsync()
    {
        // 削除してよいか確認する。
        if (_view.Confirm())
        {
            // 保管ディレクトリを丸ごと削除する。
            if (Directory.Exists(RecordController.RecordDirectoryName))
            {
                Directory.Delete(RecordController.RecordDirectoryName, true);
            }
        }
        return Task.CompletedTask;
    }
}