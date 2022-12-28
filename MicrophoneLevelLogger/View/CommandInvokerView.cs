using MicrophoneLevelLogger.Command;
using Sharprompt;

namespace MicrophoneLevelLogger.View;

public class CommandInvokerView : MicrophoneView, ICommandInvokerView
{
    public string SelectCommand(IEnumerable<string> commands)
    {
        return Prompt.Select(
            "コマンドを選択してください。",
            commands);
    }
}