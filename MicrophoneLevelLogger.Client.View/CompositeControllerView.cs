using MicrophoneLevelLogger.Client.Controller;
using Sharprompt;

namespace MicrophoneLevelLogger.Client.View;

public class CompositeControllerView : ICompositeControllerView
{
    public IController SelectController(IList<IController> controllers)
    {
        var selected = Prompt.Select("詳細コマンドを選択してください。", controllers.Select(x => x.Name));
        return controllers.Single(x => x.Name == selected);
    }
}