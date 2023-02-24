namespace MicrophoneLevelLogger.Client.Controller;

/// <summary>
/// 複合コントローラービュー
/// </summary>
public interface ICompositeControllerView
{
    /// <summary>
    /// コントローラーを選択する。
    /// </summary>
    /// <param name="composite"></param>
    /// <param name="controller"></param>
    /// <returns></returns>
    bool TrySelectController(CompositeController composite, out IController controller);
}