using MicrophoneLevelLogger.Client.Controller;
using Sharprompt;

namespace MicrophoneLevelLogger.Client.View;

public class CommandInvokerView : MicrophoneView, ICommandInvokerView
{
    public string SelectCommand(IEnumerable<string> commands)
    {
        return Prompt.Select(
            "コマンドを選択してください。",
            commands);
    }
}