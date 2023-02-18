using MicrophoneLevelLogger.Client.Controller;
using Sharprompt;

namespace MicrophoneLevelLogger.Client.View;

public class CompositeControllerView : ICompositeControllerView
{
    public bool TrySelectController(IList<IController> controllers, out IController controller)
    {
        var items = controllers.Select(x => x.Name).ToList();
        items.Add("Exit");
        var selected = Prompt.Select("詳細コマンドを選択してください。", items);
        var selectedController = controllers.SingleOrDefault(x => x.Name == selected);
        if (selectedController is not null)
        {
            controller = selectedController;
            return true;
        }

        controller = default!;
        return false;
    }
}