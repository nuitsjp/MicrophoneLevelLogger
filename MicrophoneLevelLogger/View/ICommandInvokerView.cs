using MicrophoneLevelLogger.Domain;
using Sharprompt;

namespace MicrophoneLevelLogger.View;

public interface ICommandInvokerView
{
    string SelectCommand(IEnumerable<string> commands);
}

public class CommandInvokerView : ICommandInvokerView
{
    public string SelectCommand(IEnumerable<string> commands)
    {
        return Prompt.Select(
            "コマンドを選択してください。",
            commands);
    }
}