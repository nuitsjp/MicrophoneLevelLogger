using MicrophoneLevelLogger.Client.Command.DeleteInputLevels;
using Sharprompt;

namespace MicrophoneLevelLogger.Client.View;

public class DeleteInputLevelsView : IDeleteInputLevelsView
{
    public bool Confirm()
    {
        return Prompt.Confirm("マイク入力音量の計測結果をすべて削除しますか？", false);
    }
}