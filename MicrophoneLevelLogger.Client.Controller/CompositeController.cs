namespace MicrophoneLevelLogger.Client.Controller;

/// <summary>
/// メニューを表現する複合コントローラー
/// </summary>
public class CompositeController : IController
{
    /// <summary>
    /// ビュー
    /// </summary>
    private readonly ICompositeControllerView _view;

    public CompositeController(
        string name,
        string description,
        ICompositeControllerView view)
    {
        Name = name;
        Description = description;
        _view = view;
    }

    public CompositeController(
        ICompositeControllerView view)
    {
        Name = string.Empty;
        Description = string.Empty;
        _view = view;
    }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// 概要
    /// </summary>
    public string Description { get; }
    /// <summary>
    /// メニューで選択可能なコントローラー
    /// </summary>
    public List<IController> Controllers { get; } = new();

    public CompositeController AddController(IController controller)
    {
        Controllers.Add(controller);
        return this;
    }

    public virtual async Task ExecuteAsync()
    {
        // メニューの終了または上位メニューへ戻るまで、メニュー選択・実行を繰り返す。
        while (true)
        {
            // メニューを選択する。
            if (_view.TrySelectController(this, out var selected))
            {
                // 選択されたメニューからコントローラーを実行する。
                await selected.ExecuteAsync();
            }
            else
            {
                break;
            }
        }
    }
}