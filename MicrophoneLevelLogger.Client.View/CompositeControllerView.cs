using MicrophoneLevelLogger.Client.Controller;
using Sharprompt;

namespace MicrophoneLevelLogger.Client.View;

public class CompositeControllerView : ICompositeControllerView
{
    public bool TrySelectController(CompositeController composite, out IController controller)
    {
        var maxLength = composite.Controllers
            .Where(x => x is not BorderController)
            .Max(x => x.Name.Length);
        var items = composite.Controllers
            .Select(x => x is BorderController 
                ? x.Name 
                : $"{x.Name.PadRight(maxLength)} : {x.Description}")
            .ToList();
        if (composite.Name.Any())
        {
            items.Add($"{"Return".PadRight(maxLength)} : 戻る。");
        }
        else
        {
            items.Add($"{"Exit".PadRight(maxLength)} : 終了する。");
        }
        Console.WriteLine();
        var selected = Prompt.Select("詳細コマンドを選択してください。", items);
        var selectedIndex = items.IndexOf(selected);
        if (selectedIndex < composite.Controllers.Count)
        {
            controller = composite.Controllers[selectedIndex];
            return true;
        }

        controller = default!;
        return false;
    }
}