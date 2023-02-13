using MicrophoneLevelLogger.Command;
using MicrophoneLevelLogger.Command.DeleteCalibrates;
using Sharprompt;

namespace MicrophoneLevelLogger.View;

public class DeleteCalibrateView : IDeleteCalibrateView
{
    public bool Confirm()
    {
        return Prompt.Confirm("マイク入力レベルの調整結果をすべて削除しますか？", false);
    }
}