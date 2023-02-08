using MicrophoneLevelLogger.Command;
using Sharprompt;

namespace MicrophoneLevelLogger.View;

public class RemoveInputLevelsView : IRemoveInputLevelsView
{
    public bool Confirm()
    {
        return Prompt.Confirm("マイク入力レベルの計測結果をすべて削除しますか？", false);
    }
}