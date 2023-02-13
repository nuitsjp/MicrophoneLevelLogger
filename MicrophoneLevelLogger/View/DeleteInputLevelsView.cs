using MicrophoneLevelLogger.Command.DeleteInputLevels;
using Sharprompt;

namespace MicrophoneLevelLogger.View;

public class DeleteInputLevelsView : IDeleteInputLevelsView
{
    public bool Confirm()
    {
        return Prompt.Confirm("マイク入力音量の計測結果をすべて削除しますか？", false);
    }
}