using MicrophoneLevelLogger.Client.Controller.DeleteCalibrates;
using Sharprompt;

namespace MicrophoneLevelLogger.Client.View;

public class DeleteCalibrateView : IDeleteCalibrateView
{
    public bool Confirm()
    {
        return Prompt.Confirm("マイク入力レベルの調整結果をすべて削除しますか？", false);
    }
}