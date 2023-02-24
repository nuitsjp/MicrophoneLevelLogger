using MicrophoneLevelLogger.Client.Controller;
using Sharprompt;

namespace MicrophoneLevelLogger.Client.View;

/// <summary>
/// 複合コントローラービュー
/// </summary>
public class CompositeControllerView : ICompositeControllerView
{
    /// <summary>
    /// コントローラーを選択する。
    /// </summary>
    /// <param name="composite"></param>
    /// <param name="controller"></param>
    /// <returns></returns>
    public bool TrySelectController(CompositeController composite, out IController controller)
    {
        // コントローラーの名称の長さの最大値を取得する。
        var maxLength = composite.Controllers
            .Where(x => x is not BorderController)
            .Max(x => x.Name.Length);
        // コントロールの名称、概略を作成し、最後に戻る（または終了する）メニューを追加する。
        var items = composite.Controllers
            .Select(x => x is BorderController 
                ? x.Name 
                : $"{x.Name.PadRight(maxLength)} : {x.Description}")
            .ToList();
        items.Add(composite.Name.Any()
            ? $"{"Return".PadRight(maxLength)} : 戻る。"
            : $"{"Exit".PadRight(maxLength)} : 終了する。");

        // コントローラーを選択する。
        Console.WriteLine();
        var selected = Prompt.Select("詳細コマンドを選択してください。", items);
        var selectedIndex = items.IndexOf(selected);
        if (selectedIndex < composite.Controllers.Count)
        {
            // 選択されたコントローラーを返す。
            controller = composite.Controllers[selectedIndex];
            return true;
        }

        // 戻る（または終了する）をされた場合。
        controller = default!;
        return false;
    }
}