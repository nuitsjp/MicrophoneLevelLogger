using MicrophoneLevelLogger.Domain;
using MicrophoneLevelLogger.View;
using NAudio.CoreAudioApi;
using System.Reflection.Emit;

namespace MicrophoneLevelLogger.Command;

public class CalibrateCommand : ConsoleAppBase
{
    private readonly IMicrophonesProvider _microphonesProvider;
    private readonly ICalibrateView _view;

    public CalibrateCommand(IMicrophonesProvider microphonesProvider, ICalibrateView view)
    {
        _microphonesProvider = microphonesProvider;
        _view = view;
    }

    public void Calibrate()
    {
        // すべてのマイクを取得する。
        using IMicrophones microphones = _microphonesProvider.Resolve();

        // 起動時情報を通知する。
        _view.NotifyMicrophonesInformation(microphones);
        if(_view.ConfirmInvoke() is false)
        {
            // 確認でNoが推されたら中断する。
            return;
        }

        // 画面に入力レベルを通知する。
        _view.StartNotifyMasterPeakValue(microphones);

        microphones.Calibrate();

        // 画面の入力レベル通知を停止する。
        _view.StopNotifyMasterPeakValue();

        //// 平均値が最小のマイクの音量を基準値とする
        //var referenceLevel = peakValuesList
        //    .Select(x => x.PeakValues.Average())
        //    .Min();

        //foreach (var peakValues in peakValuesList)
        //{
        //    var microphone = peakValues.Microphone;
        //    var value = peakValues.PeakValues.Average();
        //    microphone.MasterVolumeLevelScalar = (float)(Math.Sqrt(referenceLevel) / Math.Sqrt(value));
        //}
        _view.NotifyCalibrated(microphones);
    }

}