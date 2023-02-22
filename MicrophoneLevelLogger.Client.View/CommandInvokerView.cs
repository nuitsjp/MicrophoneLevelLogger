using MicrophoneLevelLogger.Client.Controller;
using Sharprompt;

namespace MicrophoneLevelLogger.Client.View;

public class CommandInvokerView : MicrophoneView, ICommandInvokerView
{
    public bool TrySelectController(IList<IController> controllers, out IController controller)
    {
        var maxLength = controllers
            .Where(x => x is not BorderController)
            .Max(x => x.Name.Length);
        var items = controllers
            .Select(x => x is BorderController
                ? x.Name
                : $"{x.Name.PadRight(maxLength)} : {x.Description}")
            .ToList();
        items.Add("Return               : 戻る。");
        Console.WriteLine();
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